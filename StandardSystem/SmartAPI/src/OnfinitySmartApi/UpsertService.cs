using System.Collections.Concurrent;
using System.Globalization;
using System.Text.Json;

namespace OnfinitySmartApi;

public enum WriteMode { Upsert, InsertOnly, UpdateOnly }

public sealed class UpsertResult
{
    public string Mode { get; init; } = "";   // created | updated | would-create | would-update
    public string Key { get; init; } = "";
    public long RecordId { get; init; }       // 0 for would-create
    public bool DryRun { get; init; }
    public IReadOnlyDictionary<string, object?>? Resolved { get; init; } // populated on dry run
    public object? Children { get; init; }     // per-collection list of created/would-create child rows
}

public sealed class ActionResult
{
    public string Action { get; init; } = "";
    public string? Key { get; init; }
    public long RecordId { get; init; }
    public string Message { get; init; } = "";
}

public sealed class ListResult
{
    public int Page { get; init; }
    public int PageSize { get; init; }
    public int Total { get; init; }
    public List<Dictionary<string, object?>> Items { get; init; } = new();
}

/// <summary>A 4xx-class problem the caller can fix (missing/ambiguous ref, validation, etc.).</summary>
public sealed class SmartApiException : Exception
{
    public int Status { get; }
    public string Code { get; }
    public SmartApiException(int status, string code, string message) : base(message)
    {
        Status = status; Code = code;
    }
}

/// <summary>
/// Resolves identifier-based payloads to FK ids + list codes, then upserts via VAAPI.
/// Supports owned sub-records (inserted first, PK fills an FK) and 1:N child collections
/// (inserted after the parent, parent FK auto-filled). Caching is process-wide with a TTL.
/// </summary>
public sealed class UpsertService
{
    private readonly VaapiClient _vaapi;
    private readonly ResolverCache _cache;

    public UpsertService(VaapiClient vaapi, ResolverCache cache)
    {
        _vaapi = vaapi;
        _cache = cache;
    }

    public async Task<UpsertResult> WriteAsync(
        EntityManifest m, Dictionary<string, JsonElement> payload, WriteMode mode, string? keyFromPath,
        bool dryRun, CancellationToken ct)
    {
        // --- key -----------------------------------------------------------
        var keyField = m.KeyField;
        string? keyVal = keyFromPath;
        if (keyVal is null && payload.TryGetValue(keyField.Api, out var ke))
            keyVal = ke.ToString();
        if (string.IsNullOrWhiteSpace(keyVal))
            throw new SmartApiException(400, "MISSING_KEY", $"Business key '{keyField.Api}' is required.");
        // make the key visible to column-building when it arrived via the URL path.
        if (!payload.ContainsKey(keyField.Api))
            payload[keyField.Api] = JsonSerializer.SerializeToElement(keyVal);

        // --- parent columns (resolves refs/lists/owned-refs) ---------------
        var cols = await BuildColumnsAsync(m.Fields, payload, mode, dryRun, ct);

        // Value used to match an existing record. A ref key (e.g. a 1:1 extension table keyed by
        // its employee) resolves to its FK id (already in cols); a scalar key is stored verbatim.
        object keyMatch;
        if (keyField.Type == "ref")
        {
            if (!cols.TryGetValue(keyField.Column, out var rk) || rk is null)
                throw new SmartApiException(422, "MISSING_KEY",
                    $"Business key '{keyField.Api}' = '{keyVal}' did not resolve to a record.");
            keyMatch = rk;
        }
        else
        {
            keyMatch = keyVal;
            cols[keyField.Column] = keyVal;
        }

        // --- existence check (optionally scoped, e.g. FiscalYear unique per calendar) ----
        var scope = "";
        foreach (var api in m.KeyScope ?? Enumerable.Empty<string>())
        {
            var sf = m.FieldByApi(api);
            if (sf is not null && cols.TryGetValue(sf.Column, out var sv) && sv is not null)
                scope += sv is string s ? $" AND {sf.Column} = '{Esc(s)}'" : $" AND {sf.Column} = {sv}";
        }
        var existingId = await FindByKeyAsync(m, keyMatch, scope, ct);
        if (existingId is null && mode == WriteMode.UpdateOnly)
            throw new SmartApiException(404, "NOT_FOUND", $"{m.Entity} '{keyVal}' does not exist.");
        if (existingId is not null && mode == WriteMode.InsertOnly)
            throw new SmartApiException(409, "ALREADY_EXISTS", $"{m.Entity} '{keyVal}' already exists.");
        if (existingId is not null) cols.Remove(keyField.Column); // don't rewrite key on update

        if (dryRun)
            return new UpsertResult
            {
                Mode = existingId is null ? "would-create" : "would-update",
                Key = keyVal,
                RecordId = existingId ?? 0,
                DryRun = true,
                Resolved = cols,
                Children = await WriteChildrenAsync(m, payload, parentId: null, cols, dryRun: true, ct)
            };

        long parentId;
        string parentMode;
        if (existingId is null)
        {
            parentId = await _vaapi.InsertRecordAsync(m.Table, cols.Keys.ToList(), cols, m.IncludeStandardCols, ct);
            parentMode = "created";
        }
        else
        {
            await _vaapi.UpdateRecordAsync(m.Table, cols.Keys.ToList(), cols, existingId.Value, ct);
            parentId = existingId.Value; parentMode = "updated";
        }

        // children are inserted after the parent (no cross-table transaction in VAAPI;
        // a child failure throws after the parent is already written - reported to the caller).
        var children = await WriteChildrenAsync(m, payload, parentId, cols, dryRun: false, ct);
        return new UpsertResult { Mode = parentMode, Key = keyVal, RecordId = parentId, Children = children };
    }

    // ---- column building (shared by parent, owned-refs and children) ------

    private async Task<Dictionary<string, object?>> BuildColumnsAsync(
        IReadOnlyList<FieldSpec> fields, Dictionary<string, JsonElement> payload, WriteMode mode,
        bool dryRun, CancellationToken ct, long? parentId = null)
    {
        var cols = new Dictionary<string, object?>(StringComparer.Ordinal);
        var byApi = new Dictionary<string, object?>(StringComparer.OrdinalIgnoreCase); // resolved value per api name
        var derived = new List<FieldSpec>();

        // pass 1: scalars / refs / lists / owned-refs
        foreach (var f in fields)
        {
            if (f.Type == "derived") { derived.Add(f); continue; }

            if (!payload.TryGetValue(f.Api, out var raw))
            {
                if (f.Required && mode != WriteMode.UpdateOnly && f.Default is null)
                    throw new SmartApiException(422, "MISSING_FIELD", $"Required field '{f.Api}' is missing.");
                if (f.Default is JsonElement def && mode == WriteMode.InsertOnly)
                    SetCol(cols, byApi, f, Scalar(f, def));
                continue;
            }
            if (raw.ValueKind == JsonValueKind.Null) { byApi[f.Api] = cols[f.Column] = null; continue; }

            SetCol(cols, byApi, f, f.Type switch
            {
                "ref"      => await ResolveRefAsync(f, raw.ToString(), ct),
                "list"     => ResolveList(f, raw.ToString()),
                "ownedRef" => await ResolveOwnedAsync(f, raw, dryRun, ct),
                _          => Scalar(f, raw)
            });
        }

        // pass 2: derived fields (look up a value from an already-resolved field's row)
        foreach (var f in derived)
        {
            var val = await ResolveDerivedAsync(f, byApi, parentId, ct);
            if (val is null)
            {
                if (f.Required && mode != WriteMode.UpdateOnly)
                    throw new SmartApiException(422, "MISSING_DERIVED",
                        $"Could not derive '{f.Api}' from '{f.Derived!.From}'.");
                continue;
            }
            cols[f.Column] = val;
        }
        return cols;
    }

    private async Task<object?> ResolveDerivedAsync(
        FieldSpec f, IReadOnlyDictionary<string, object?> byApi, long? parentId, CancellationToken ct)
    {
        var d = f.Derived ?? throw new SmartApiException(500, "NO_DERIVED",
            $"Field '{f.Api}' is derived but has no 'derived' spec.");
        object? key = string.Equals(d.From, "$parent", StringComparison.OrdinalIgnoreCase)
            ? parentId
            : (byApi.TryGetValue(d.From, out var k) ? k : null);
        if (key is null) return null; // source field not supplied / not resolved

        var keyStr = Convert.ToString(key, CultureInfo.InvariantCulture) ?? "";
        var where = $"{d.WhereColumn} = '{Esc(keyStr)}'";
        if (!string.IsNullOrWhiteSpace(d.ExtraWhere)) where += $" AND {d.ExtraWhere}";
        where += " AND IsActive = 'Y'";
        var rows = await _vaapi.GetRecordsAsync(d.Table, $"SELECT {d.SelectColumn} FROM {d.Table} WHERE {where}", ct);
        if (rows.Count == 0) return null; // tolerant: first match wins if several
        return ReadId(rows[0], d.SelectColumn) is long id
            ? id
            : (rows[0].TryGetValue(d.SelectColumn, out var v) ? v.ToString() : null);
    }

    private async Task<object?> ResolveOwnedAsync(FieldSpec f, JsonElement raw, bool dryRun, CancellationToken ct)
    {
        if (f.Owned is null)
            throw new SmartApiException(500, "NO_OWNED", $"Field '{f.Api}' is ownedRef but has no 'owned' spec.");
        if (raw.ValueKind != JsonValueKind.Object)
            throw new SmartApiException(422, "BAD_OWNED", $"'{f.Api}' must be an object.");

        var nested = ToDict(raw);
        var ownedCols = await BuildColumnsAsync(f.Owned.Fields, nested, WriteMode.InsertOnly, dryRun, ct);
        if (dryRun)
            return new Dictionary<string, object?> { ["__create"] = f.Owned.Table, ["values"] = ownedCols };
        return await _vaapi.InsertRecordAsync(f.Owned.Table, ownedCols.Keys.ToList(), ownedCols, f.Owned.IncludeStandardCols, ct);
    }

    private async Task<Dictionary<string, object?>?> WriteChildrenAsync(
        EntityManifest m, Dictionary<string, JsonElement> payload, long? parentId,
        IReadOnlyDictionary<string, object?> parentCols, bool dryRun, CancellationToken ct)
    {
        if (m.Children is null || m.Children.Count == 0) return null;
        var result = new Dictionary<string, object?>();
        foreach (var (childKey, spec) in m.Children)
        {
            if (!payload.TryGetValue(childKey, out var arr)) continue;
            if (arr.ValueKind != JsonValueKind.Array)
                throw new SmartApiException(422, "BAD_CHILD", $"'{childKey}' must be an array.");

            var items = new List<object?>();
            foreach (var el in arr.EnumerateArray())
            {
                if (el.ValueKind != JsonValueKind.Object)
                    throw new SmartApiException(422, "BAD_CHILD", $"Each '{childKey}' item must be an object.");

                var childDict = ToDict(el);
                var childCols = await BuildColumnsAsync(spec.Fields, childDict, WriteMode.InsertOnly, dryRun, ct, parentId);
                childCols[spec.ParentLink] = (object?)parentId ?? "(parent)";

                // optional hierarchical placement: child payload supplies the parent record's identifier
                string? treeParent = spec.TreeNode is { } tn0 && childDict.TryGetValue(tn0.ParentField, out var pv)
                                     && pv.ValueKind is not JsonValueKind.Null ? pv.ToString() : null;

                if (dryRun)
                {
                    var entry = new Dictionary<string, object?> { ["__create"] = spec.Table, ["values"] = childCols };
                    if (treeParent is not null) entry["treeNodeUnder"] = treeParent;
                    items.Add(entry);
                }
                else
                {
                    var id = await _vaapi.InsertRecordAsync(spec.Table, childCols.Keys.ToList(), childCols, spec.IncludeStandardCols, ct);
                    var entry = new Dictionary<string, object?> { ["table"] = spec.Table, ["id"] = id };
                    if (treeParent is not null && spec.TreeNode is { } tn)
                    {
                        await PlaceTreeNodeAsync(tn, m, parentCols, parentId, id, treeParent, ct);
                        entry["treeNodeUnder"] = treeParent;
                    }
                    items.Add(entry);
                }
            }
            result[childKey] = items;
        }
        return result.Count == 0 ? null : result;
    }

    // Move a freshly-inserted, tree-supported record's node under a parent node in its tree.
    private async Task PlaceTreeNodeAsync(
        TreeNodeSpec tn, EntityManifest m, IReadOnlyDictionary<string, object?> parentCols,
        long? scopeParentId, long nodeId, string parentValue, CancellationToken ct)
    {
        var r = tn.ParentRef ?? throw new SmartApiException(500, "NO_PARENTREF", "treeNode has no parentRef.");
        var where = $"{r.MatchOn} = '{Esc(parentValue)}'";
        if (tn.ScopeColumn is not null && scopeParentId is long sp) where += $" AND {tn.ScopeColumn} = {sp}";
        where += " AND IsActive = 'Y'";
        var rows = await _vaapi.GetRecordsAsync(r.Table, $"SELECT {r.Returns} FROM {r.Table} WHERE {where}", ct);
        if (rows.Count != 1)
            throw new SmartApiException(422, "MISSING_TREE_PARENT",
                $"Tree parent '{parentValue}' matched {rows.Count} rows in {r.Table}.{r.MatchOn}.");
        var parentNodeId = ReadId(rows[0], r.Returns)!.Value;

        var tf = m.FieldByApi(tn.TreeFrom);
        if (tf is null || !parentCols.TryGetValue(tf.Column, out var tv) || tv is not long treeId)
            throw new SmartApiException(422, "NO_TREE", $"Could not determine the tree from '{tn.TreeFrom}'.");

        await _vaapi.UpdateByQueryAsync(tn.NodeTable,
            $"UPDATE {tn.NodeTable} SET {tn.ParentColumn} = {parentNodeId} " +
            $"WHERE {tn.NodeColumn} = {nodeId} AND {tn.TreeColumn} = {treeId}", ct);
    }

    // ---- retrieval (list / read) -----------------------------------------

    private static readonly string[] ReadableTypes = { "string", "number", "boolean", "ref", "list" };

    /// <summary>List/search records by identifier filters, paged, sorted, optionally field-projected.</summary>
    public async Task<ListResult> ListAsync(
        EntityManifest m, IReadOnlyDictionary<string, string> filters, int page, int pageSize,
        bool includeIds, string? sort, string? select, CancellationToken ct)
    {
        var fields = SelectFields(m, select);

        var where = new List<string> { "IsActive = 'Y'" };
        foreach (var (name, raw) in filters)
        {
            var f = m.FieldByApi(name)
                ?? throw new SmartApiException(400, "UNKNOWN_FILTER", $"'{name}' is not a field of {m.Entity}.");
            where.Add(await FilterClauseAsync(f, raw, ct));
        }
        var whereSql = string.Join(" AND ", where);
        var orderBy = BuildOrderBy(m, sort);

        var countRows = await _vaapi.GetRecordsAsync(m.Table,
            $"SELECT COUNT(*) AS n FROM {m.Table} WHERE {whereSql}", ct);
        int total = countRows.Count > 0 && countRows[0].TryGetValue("n", out var nv)
                    && nv.ValueKind == JsonValueKind.Number ? (int)nv.GetDecimal() : 0;

        var cols = ReadColumns(m.Pk, m.KeyField.Column, fields);
        var rows = await _vaapi.GetRecordsPagedAsync(m.Table,
            $"SELECT {string.Join(", ", cols)} FROM {m.Table} WHERE {whereSql}{orderBy}", page, pageSize, ct);

        var items = rows.Select(r => MapRow(m.Pk, m.KeyField.Column, fields, r)).ToList();
        await ReverseRefsAsync(fields, rows, items, includeIds, ct);
        return new ListResult { Page = page, PageSize = pageSize, Total = total, Items = items };
    }

    /// <summary>Read one record by key (refs/lists as identifiers), optionally with its children.</summary>
    public async Task<Dictionary<string, object?>?> ReadAsync(
        EntityManifest m, string key, bool includeChildren, bool includeIds, string? select, CancellationToken ct)
    {
        var fields = SelectFields(m, select);
        var cols = ReadColumns(m.Pk, m.KeyField.Column, fields);
        var rows = await _vaapi.GetRecordsAsync(m.Table,
            $"SELECT {string.Join(", ", cols)} FROM {m.Table} WHERE {m.KeyField.Column} = '{Esc(key)}' AND IsActive = 'Y'", ct);
        if (rows.Count == 0) return null;

        var items = new List<Dictionary<string, object?>> { MapRow(m.Pk, m.KeyField.Column, fields, rows[0]) };
        await ReverseRefsAsync(fields, rows, items, includeIds, ct);
        var item = items[0];

        if (includeChildren && m.Children is { Count: > 0 } && ReadId(rows[0], m.Pk) is long pid)
        {
            var ch = new Dictionary<string, object?>();
            foreach (var (cname, spec) in m.Children)
                ch[cname] = await ReadChildrenAsync(spec, pid, includeIds, ct);
            item["children"] = ch;
        }
        return item;
    }

    private async Task<List<Dictionary<string, object?>>> ReadChildrenAsync(
        ChildSpec spec, long parentId, bool includeIds, CancellationToken ct)
    {
        var fields = spec.Fields.Where(f => ReadableTypes.Contains(f.Type)).ToList();
        var cols = new List<string> { spec.Pk };
        foreach (var f in fields) if (!cols.Contains(f.Column)) cols.Add(f.Column);
        var rows = await _vaapi.GetRecordsAsync(spec.Table,
            $"SELECT {string.Join(", ", cols)} FROM {spec.Table} WHERE {spec.ParentLink} = {parentId} AND IsActive = 'Y'", ct);

        var items = rows.Select(r =>
        {
            var it = new Dictionary<string, object?> { ["recordId"] = ReadId(r, spec.Pk) };
            foreach (var f in fields)
            {
                if (f.Type == "ref" || !r.TryGetValue(f.Column, out var v) || v.ValueKind == JsonValueKind.Null) continue;
                it[f.Api] = f.Type switch
                {
                    "boolean" => string.Equals(v.ToString(), "Y", StringComparison.OrdinalIgnoreCase),
                    "list"    => ReverseList(f, v.ToString()),
                    _         => v.ToString()
                };
            }
            return it;
        }).ToList();
        await ReverseRefsAsync(fields, rows, items, includeIds, ct);
        return items;
    }

    private async Task<string> FilterClauseAsync(FieldSpec f, string raw, CancellationToken ct) => f.Type switch
    {
        "ref"     => $"{f.Column} = {await ResolveRefAsync(f, raw, ct)}",
        "list"    => $"{f.Column} = '{Esc(ResolveList(f, raw))}'",
        "boolean" => $"{f.Column} = '{(raw is "true" or "1" or "Y" or "y" ? "Y" : "N")}'",
        "number"  => decimal.TryParse(raw, NumberStyles.Any, CultureInfo.InvariantCulture, out var d)
                        ? $"{f.Column} = {d}"
                        : throw new SmartApiException(400, "BAD_FILTER", $"'{f.Api}' must be a number."),
        _         => raw.EndsWith('*') ? $"{f.Column} LIKE '{Esc(raw[..^1])}%'" : $"{f.Column} = '{Esc(raw)}'"
    };

    // restrict the readable field set to ?fields=a,b,c (key/recordId are always returned anyway).
    private static List<FieldSpec> SelectFields(EntityManifest m, string? select)
    {
        var all = m.Fields.Where(f => ReadableTypes.Contains(f.Type)).ToList();
        if (string.IsNullOrWhiteSpace(select)) return all;
        var want = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach (var s in select.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
        {
            if (m.FieldByApi(s) is null && !s.Equals("key", StringComparison.OrdinalIgnoreCase)
                                        && !s.Equals("recordId", StringComparison.OrdinalIgnoreCase))
                throw new SmartApiException(400, "UNKNOWN_FIELD", $"'{s}' is not a field of {m.Entity}.");
            want.Add(s);
        }
        return all.Where(f => want.Contains(f.Api)).ToList();
    }

    // ?sort=name,-createdOn  ('-' = descending). Sorts by the stored column (refs by id, lists by code).
    private static string BuildOrderBy(EntityManifest m, string? sort)
    {
        if (string.IsNullOrWhiteSpace(sort)) return "";
        var parts = new List<string>();
        foreach (var raw in sort.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
        {
            var desc = raw[0] == '-';
            var name = (raw[0] is '-' or '+') ? raw[1..] : raw;
            var f = m.FieldByApi(name)
                ?? throw new SmartApiException(400, "BAD_SORT", $"Cannot sort by unknown field '{name}'.");
            parts.Add($"{f.Column} {(desc ? "DESC" : "ASC")}");
        }
        return parts.Count > 0 ? " ORDER BY " + string.Join(", ", parts) : "";
    }

    private static List<string> ReadColumns(string pk, string keyCol, IEnumerable<FieldSpec> fields)
    {
        var cols = new List<string> { pk };
        if (keyCol != pk) cols.Add(keyCol);
        foreach (var f in fields) if (!cols.Contains(f.Column)) cols.Add(f.Column);
        return cols;
    }

    private static Dictionary<string, object?> MapRow(
        string pk, string keyCol, IEnumerable<FieldSpec> fields, Dictionary<string, JsonElement> r)
    {
        var item = new Dictionary<string, object?>
        {
            ["key"] = r.TryGetValue(keyCol, out var k) ? k.ToString() : null,
            ["recordId"] = ReadId(r, pk),
        };
        foreach (var f in fields)
        {
            if (f.IsKey) continue;
            if (f.Type == "ref") continue; // filled by ReverseRefsAsync
            if (!r.TryGetValue(f.Column, out var v) || v.ValueKind == JsonValueKind.Null) continue;
            item[f.Api] = f.Type switch
            {
                "boolean" => string.Equals(v.ToString(), "Y", StringComparison.OrdinalIgnoreCase),
                "list"    => ReverseList(f, v.ToString()),
                _         => v.ToString()
            };
        }
        return item;
    }

    // batch-resolve every ref field on the page: one lookup per ref-table (no N+1 per row).
    private async Task ReverseRefsAsync(
        List<FieldSpec> fields, List<Dictionary<string, JsonElement>> rows,
        List<Dictionary<string, object?>> items, bool includeIds, CancellationToken ct)
    {
        foreach (var f in fields.Where(f => f.Type == "ref" && f.Ref is not null))
        {
            var r = f.Ref!;
            var ids = new HashSet<long>();
            foreach (var row in rows)
                if (ReadId(row, f.Column) is long id) ids.Add(id);
            if (ids.Count == 0) continue;

            var lookup = await _vaapi.GetRecordsAsync(r.Table,
                $"SELECT {r.Returns}, {r.MatchOn} FROM {r.Table} WHERE {r.Returns} IN ({string.Join(",", ids)})", ct);
            var map = new Dictionary<long, string?>();
            foreach (var lr in lookup)
                if (ReadId(lr, r.Returns) is long lid)
                    map[lid] = lr.TryGetValue(r.MatchOn, out var nm) ? nm.ToString() : null;

            for (int i = 0; i < rows.Count; i++)
                if (ReadId(rows[i], f.Column) is long id)
                {
                    items[i][f.Api] = map.TryGetValue(id, out var name) ? name : id;
                    if (includeIds) items[i][f.Api + "Id"] = id;
                }
        }
    }

    private static string ReverseList(FieldSpec f, string code)
    {
        if (f.List is not null)
            foreach (var kv in f.List)
                if (kv.Value == code) return kv.Key;
        return code;
    }

    // ---- window processes (actions) --------------------------------------

    public async Task<ActionResult> RunActionAsync(
        EntityManifest m, string key, string actionName, Dictionary<string, JsonElement> body, CancellationToken ct)
    {
        if (m.Actions is null || !m.Actions.TryGetValue(actionName, out var a))
            throw new SmartApiException(404, "UNKNOWN_ACTION", $"{m.Entity} has no action '{actionName}'.");
        if (string.IsNullOrEmpty(a.SearchKey) && a.ProcessId is null)
            throw new SmartApiException(500, "BAD_ACTION", $"Action '{actionName}' has no searchKey or processId.");

        long recordId = 0;
        if (!a.Menu)
        {
            var scope = await BuildScopeAsync(m, body, ct); // honour keyScope (e.g. FiscalYear unique per calendar)
            recordId = await FindByKeyAsync(m, key, scope, ct)
                       ?? throw new SmartApiException(404, "NOT_FOUND", $"{m.Entity} '{key}' does not exist.");
        }

        var prms = new List<KeyValuePair<string, object?>>();
        foreach (var p in a.Params ?? new())
        {
            object? val;
            if (body.TryGetValue(p.Key, out var raw) && raw.ValueKind != JsonValueKind.Null)
                val = p.Type switch
                {
                    "ref"  => await ResolveRefAsync(new FieldSpec { Api = p.Name, Ref = p.Ref }, raw.ToString(), ct),
                    "list" => p.List is not null ? ResolveList(new FieldSpec { Api = p.Name, List = p.List }, raw.ToString()) : raw.ToString(),
                    _      => JsonToObj(raw)
                };
            else if (p.Const is JsonElement c) val = JsonToObj(c);
            else if (p.Required) throw new SmartApiException(422, "MISSING_PARAM", $"Action '{actionName}' requires '{p.Key}'.");
            else continue;
            prms.Add(new(p.Name, val));
        }

        var msg = await _vaapi.RunProcessAsync(a.SearchKey, a.ProcessId, recordId, prms, ct);
        return new ActionResult { Action = actionName, Key = a.Menu ? null : key, RecordId = recordId, Message = msg };
    }

    private static object? JsonToObj(JsonElement v) => v.ValueKind switch
    {
        JsonValueKind.String => v.GetString(),
        JsonValueKind.Number => v.TryGetInt64(out var n) ? n : v.GetDecimal(),
        JsonValueKind.True  => "Y",
        JsonValueKind.False => "N",
        _ => v.ToString()
    };

    // ---- resolution primitives -------------------------------------------

    // Build a key-scope SQL fragment from the request body (used by record-bound actions on keyScoped entities).
    private async Task<string> BuildScopeAsync(EntityManifest m, Dictionary<string, JsonElement> body, CancellationToken ct)
    {
        var scope = "";
        foreach (var api in m.KeyScope ?? Enumerable.Empty<string>())
        {
            var f = m.FieldByApi(api);
            if (f is null || !body.TryGetValue(api, out var raw) || raw.ValueKind == JsonValueKind.Null) continue;
            object? val = f.Type switch
            {
                "ref"  => await ResolveRefAsync(f, raw.ToString(), ct),
                "list" => ResolveList(f, raw.ToString()),
                _      => raw.ValueKind == JsonValueKind.String ? raw.GetString() : raw.ToString()
            };
            scope += val is string s ? $" AND {f.Column} = '{Esc(s)}'" : $" AND {f.Column} = {val}";
        }
        return scope;
    }

    private async Task<long?> FindByKeyAsync(EntityManifest m, object key, string scope, CancellationToken ct)
    {
        var keyClause = key is string ks
            ? $"'{Esc(ks)}'"
            : Convert.ToString(key, CultureInfo.InvariantCulture);
        var sql = $"SELECT {m.Pk} FROM {m.Table} WHERE {m.KeyField.Column} = {keyClause}{scope} AND IsActive = 'Y'";
        var rows = await _vaapi.GetRecordsAsync(m.Table, sql, ct);
        if (rows.Count == 0) return null;
        if (rows.Count > 1)
            throw new SmartApiException(409, "AMBIGUOUS_KEY", $"Key '{key}' matches {rows.Count} {m.Entity} records.");
        return ReadId(rows[0], m.Pk);
    }

    private async Task<long> ResolveRefAsync(FieldSpec f, string value, CancellationToken ct)
    {
        var r = f.Ref!;
        var cacheKey = $"{r.Table}|{r.MatchOn}|{value}";
        if (_cache.TryGet(cacheKey, out var cached)) return cached;

        var sql = $"SELECT {r.Returns} FROM {r.Table} WHERE {r.MatchOn} = '{Esc(value)}' AND IsActive = 'Y'";
        var rows = await _vaapi.GetRecordsAsync(r.Table, sql, ct);
        if (rows.Count == 0)
            throw new SmartApiException(422, "MISSING_REF",
                $"'{f.Api}' = '{value}' not found in {r.Table}.{r.MatchOn}.");
        if (rows.Count > 1)
            throw new SmartApiException(422, "AMBIGUOUS_REF",
                $"'{f.Api}' = '{value}' matches {rows.Count} rows in {r.Table}.{r.MatchOn}.");

        var id = ReadId(rows[0], r.Returns)
                 ?? throw new SmartApiException(502, "BAD_REF_RESULT",
                        $"{r.Table}.{r.Returns} did not return a numeric id.");
        _cache.Set(cacheKey, id);
        return id;
    }

    private static string ResolveList(FieldSpec f, string label)
    {
        if (f.List is null)
            throw new SmartApiException(500, "NO_LIST", $"Field '{f.Api}' has no list map.");
        if (f.List.TryGetValue(label, out var code)) return code;
        var hit = f.List.FirstOrDefault(kv =>
            string.Equals(kv.Key, label, StringComparison.OrdinalIgnoreCase));
        if (hit.Key is not null) return hit.Value;
        var allowed = string.Join(", ", f.List.Keys.Where(k => !k.StartsWith('_')));
        throw new SmartApiException(422, "BAD_LIST_VALUE",
            $"'{f.Api}' = '{label}' is not valid. Allowed: {allowed}.");
    }

    private static object? Scalar(FieldSpec f, JsonElement v)
    {
        switch (f.Type)
        {
            case "boolean":
                var b = v.ValueKind == JsonValueKind.True
                        || (v.ValueKind == JsonValueKind.String && bool.TryParse(v.GetString(), out var pb) && pb);
                return b ? "Y" : "N"; // VA stores Yes-No as Y/N
            case "number":
                if (v.ValueKind == JsonValueKind.Number) return v.GetDecimal();
                if (v.ValueKind == JsonValueKind.String &&
                    decimal.TryParse(v.GetString(), NumberStyles.Any, CultureInfo.InvariantCulture, out var d))
                    return d;
                throw new SmartApiException(422, "BAD_NUMBER", $"'{f.Api}' must be a number.");
            default:
                var s = v.ValueKind == JsonValueKind.String ? v.GetString() ?? "" : v.ToString();
                if (f.MaxLen is int max && s.Length > max)
                    throw new SmartApiException(422, "TOO_LONG", $"'{f.Api}' exceeds {max} chars.");
                return s;
        }
    }

    // Set a field's column (and track it by api name); mirror into any alsoColumns.
    private static void SetCol(Dictionary<string, object?> cols, Dictionary<string, object?> byApi, FieldSpec f, object? val)
    {
        byApi[f.Api] = cols[f.Column] = val;
        if (f.AlsoColumns is { } extra)
            foreach (var c in extra) cols[c] = val;
    }

    private static Dictionary<string, JsonElement> ToDict(JsonElement obj)
    {
        var d = new Dictionary<string, JsonElement>(StringComparer.OrdinalIgnoreCase);
        foreach (var p in obj.EnumerateObject()) d[p.Name] = p.Value;
        return d;
    }

    private static long? ReadId(Dictionary<string, JsonElement> row, string col)
    {
        if (!row.TryGetValue(col, out var v)) return null;
        if (v.ValueKind == JsonValueKind.Number && v.TryGetInt64(out var n)) return n;
        if (v.ValueKind == JsonValueKind.String && long.TryParse(v.GetString(), out var sn)) return sn;
        return null;
    }

    // Minimal defense for values interpolated into a VAAPI selectQuery.
    private static string Esc(string s) => s.Replace("'", "''");
}

/// <summary>Process-wide TTL cache for resolved identifier -> id lookups.</summary>
public sealed class ResolverCache
{
    private readonly record struct Entry(long Id, DateTimeOffset Expires);
    private readonly ConcurrentDictionary<string, Entry> _map = new();
    private readonly TimeSpan _ttl;

    public ResolverCache(IConfiguration cfg)
        => _ttl = TimeSpan.FromSeconds(cfg.GetValue("SmartApi:ResolverCacheSeconds", 300));

    public bool TryGet(string key, out long id)
    {
        if (_map.TryGetValue(key, out var e) && e.Expires > DateTimeOffset.UtcNow) { id = e.Id; return true; }
        id = 0; return false;
    }

    public void Set(string key, long id) => _map[key] = new Entry(id, DateTimeOffset.UtcNow + _ttl);
}
