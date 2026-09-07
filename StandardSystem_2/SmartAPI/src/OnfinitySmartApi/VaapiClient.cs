using System.Net.Http.Json;
using System.Text.Json;

namespace OnfinitySmartApi;

/// <summary>
/// Per-request holder for the caller's VAAPI key pair. Populated from inbound headers
/// (accessKey / secretKey) and forwarded verbatim to VAAPI — the "pass-through" auth model.
/// </summary>
public sealed class VaapiCredentials
{
    public string? AccessKey { get; set; }
    public string? SecretKey { get; set; }
    public bool IsPresent => !string.IsNullOrEmpty(AccessKey) && !string.IsNullOrEmpty(SecretKey);
}

public sealed class VaapiException : Exception
{
    public VaapiException(string message) : base(message) { }
}

/// <summary>
/// Thin typed client over the three VAAPI methods the Smart API needs.
/// Contract per ToolLibrary_VAAPI_Draft.md / VAAPI Services Document v2.0.
/// </summary>
public sealed class VaapiClient
{
    private readonly HttpClient _http;
    private readonly VaapiCredentials _cred;
    private static readonly JsonSerializerOptions J = new() { PropertyNameCaseInsensitive = true };

    public VaapiClient(HttpClient http, VaapiCredentials cred)
    {
        _http = http;
        _cred = cred;
    }

    private HttpRequestMessage Build(string path, object body)
    {
        var req = new HttpRequestMessage(HttpMethod.Post, path.TrimStart('/'))
        {
            Content = JsonContent.Create(body)
        };
        req.Headers.TryAddWithoutValidation("accessKey", _cred.AccessKey);
        req.Headers.TryAddWithoutValidation("secretKey", _cred.SecretKey);
        return req;
    }

    /// <summary>GetRecordV2 — SELECT rows. Returns the `data` array (column keys are lowercased by VAAPI).</summary>
    public async Task<List<Dictionary<string, JsonElement>>> GetRecordsAsync(
        string tableName, string selectQuery, CancellationToken ct = default)
    {
        using var resp = await _http.SendAsync(
            Build("GetRecordV2", new { tableName, selectQuery }), ct);
        return ParseRows(await ReadAsync(resp, ct));
    }

    /// <summary>GetRecordV3 - paged SELECT (GetRecordV2 cannot use LIMIT/FETCH; V3 handles paging).</summary>
    public async Task<List<Dictionary<string, JsonElement>>> GetRecordsPagedAsync(
        string tableName, string selectQuery, int pageNo, int pageSize, CancellationToken ct = default)
    {
        using var resp = await _http.SendAsync(
            Build("GetRecordV3", new { tableName, selectQuery, pageNo, pageSize }), ct);
        return ParseRows(await ReadAsync(resp, ct));
    }

    private static List<Dictionary<string, JsonElement>> ParseRows(JsonDocument doc)
    {
        var rows = new List<Dictionary<string, JsonElement>>();
        if (doc.RootElement.TryGetProperty("data", out var data) && data.ValueKind == JsonValueKind.Array)
            foreach (var row in data.EnumerateArray())
            {
                var map = new Dictionary<string, JsonElement>(StringComparer.OrdinalIgnoreCase);
                foreach (var p in row.EnumerateObject()) map[p.Name] = p.Value.Clone();
                rows.Add(map);
            }
        return rows;
    }

    /// <summary>InsertRecordM3 — insert one record. Returns the new PK.</summary>
    public async Task<long> InsertRecordAsync(
        string tableName, IReadOnlyList<string> colName,
        IReadOnlyDictionary<string, object?> values, bool includeStandardCols, CancellationToken ct = default)
    {
        // M3 expects `values` as a FLAT array aligned positionally to colName (verified live on erplive).
        var flat = colName.Select(c => values[c]).ToArray();
        var body = new
        {
            colName,
            tableName,
            values = flat,
            includeStandardCols,
            fileName = "",
            saveFileInDisk = false
        };
        using var resp = await _http.SendAsync(Build("InsertRecordM3", body), ct);
        var doc = await ReadAsync(resp, ct);
        return ExtractPk(doc, tableName);
    }

    /// <summary>UpdateRecordM3 — update one record by PK.</summary>
    public async Task UpdateRecordAsync(
        string tableName, IReadOnlyList<string> colName,
        IReadOnlyDictionary<string, object?> values, long recordId, CancellationToken ct = default)
    {
        var flat = colName.Select(c => values[c]).ToArray(); // flat array aligned to colName
        var body = new { tableName, colName, values = flat, record_ID = recordId };
        using var resp = await _http.SendAsync(Build("UpdateRecordM3", body), ct);
        await ReadAsync(resp, ct); // throws on failure
    }

    /// <summary>Run a raw UPDATE/DELETE (UpdateORDeleteRecord) - used for composite-key tables (e.g. tree nodes).</summary>
    public async Task UpdateByQueryAsync(string tableName, string updateSql, CancellationToken ct = default)
    {
        using var resp = await _http.SendAsync(
            Build("UpdateORDeleteRecord", new { pid = 0, selectQuery = updateSql, tableName }), ct);
        var doc = await ReadAsync(resp, ct);
        var data = doc.RootElement.TryGetProperty("data", out var d) ? d.ToString() : "";
        if (data.Contains("NotUpdated", StringComparison.OrdinalIgnoreCase))
            throw new VaapiException($"Update failed ({tableName}): {Trim(data)}");
    }

    /// <summary>RunProcessBySearchKey (or RunProcess if processId given) on a record. Returns the process message.</summary>
    public async Task<string> RunProcessAsync(
        string? searchKey, int? processId, long recordId,
        IReadOnlyList<KeyValuePair<string, object?>> prms, CancellationToken ct = default)
    {
        var param = prms.Select(p => new { p.Key, p.Value })
                        .Select(p => new Dictionary<string, object?> { ["Name"] = p.Key, ["Value"] = p.Value })
                        .ToArray();
        HttpRequestMessage req;
        if (processId is int pid)
            req = Build("RunProcess", new { AD_Process_ID = pid, Record_ID = recordId, Param = param });
        else
            req = Build("RunProcessBySearchKey", new { SearchKey = searchKey, Record_ID = recordId, Param = param });

        using var resp = await _http.SendAsync(req, ct);
        var doc = await ReadAsync(resp, ct);
        var root = doc.RootElement;
        var result = root.TryGetProperty("result", out var r) ? r.GetString() : null;
        var msg = root.TryGetProperty("data", out var d) ? d.ToString() : "";
        if (!string.Equals(result, "Success", StringComparison.OrdinalIgnoreCase))
            throw new VaapiException($"Process failed ({searchKey ?? processId?.ToString()}): {Trim(msg)}");
        return msg;
    }

    private static async Task<JsonDocument> ReadAsync(HttpResponseMessage resp, CancellationToken ct)
    {
        var raw = await resp.Content.ReadAsStringAsync(ct);
        if (!resp.IsSuccessStatusCode)
            throw new VaapiException($"VAAPI HTTP {(int)resp.StatusCode}: {Trim(raw)}");
        try { return JsonDocument.Parse(string.IsNullOrWhiteSpace(raw) ? "{}" : raw); }
        catch (JsonException) { throw new VaapiException($"VAAPI returned non-JSON: {Trim(raw)}"); }
    }

    private static long ExtractPk(JsonDocument doc, string tableName)
    {
        // Shape: {"id":<pk|-1>,"identifier":"...","code":200,"result":"Success","data":<pk|0|"NotInserted ..."> }
        // A real insert => positive id/data. Failure => id==-1 and data is 0 / "0" / a "NotInserted ..." message.
        var root = doc.RootElement;
        if (root.ValueKind == JsonValueKind.Object && root.TryGetProperty("data", out var data))
        {
            if (data.ValueKind == JsonValueKind.String)
            {
                var s = data.GetString() ?? "";
                if (s.Contains("NotInserted", StringComparison.OrdinalIgnoreCase) ||
                    s.Contains("mandatory", StringComparison.OrdinalIgnoreCase))
                    throw new VaapiException($"VAAPI did not insert ({tableName}): {s}");
                if (long.TryParse(s, out var sn))
                {
                    if (sn > 0) return sn;
                    throw new VaapiException($"VAAPI did not insert ({tableName}): the model rejected the record (returned {sn}).");
                }
            }
            else if (data.ValueKind == JsonValueKind.Number && data.TryGetInt64(out var dn))
            {
                if (dn > 0) return dn;
                throw new VaapiException($"VAAPI did not insert ({tableName}): the model rejected the record (returned {dn}).");
            }
        }
        if (root.ValueKind == JsonValueKind.Object && root.TryGetProperty("id", out var idEl)
            && idEl.ValueKind == JsonValueKind.Number && idEl.TryGetInt64(out var idn) && idn > 0)
            return idn;
        if (root.ValueKind == JsonValueKind.Number && root.TryGetInt64(out var direct) && direct > 0) return direct;
        throw new VaapiException($"Could not read inserted PK ({tableName}): {Trim(root.GetRawText())}");
    }

    private static string Trim(string s) => s.Length > 500 ? s[..500] + "…" : s;
}
