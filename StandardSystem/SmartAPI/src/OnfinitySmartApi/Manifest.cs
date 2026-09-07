using System.Collections.Concurrent;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace OnfinitySmartApi;

// ---- Manifest POCOs (shape of manifests/*.json) ----------------------------

public sealed class EntityManifest
{
    [JsonPropertyName("entity")] public string Entity { get; set; } = "";
    [JsonPropertyName("table")] public string Table { get; set; } = "";
    [JsonPropertyName("pk")] public string Pk { get; set; } = "";
    [JsonPropertyName("key")] public KeySpec Key { get; set; } = new();
    [JsonPropertyName("identifierColumns")] public List<string> IdentifierColumns { get; set; } = new();
    // extra fields whose resolved values scope the key's uniqueness (e.g. FiscalYear is unique per calendar).
    [JsonPropertyName("keyScope")] public List<string>? KeyScope { get; set; }
    [JsonPropertyName("includeStandardCols")] public bool IncludeStandardCols { get; set; } = true;
    [JsonPropertyName("fields")] public List<FieldSpec> Fields { get; set; } = new();
    // 1:N child collections written after the parent (parentLink FK auto-filled from the new PK).
    [JsonPropertyName("children")] public Dictionary<string, ChildSpec>? Children { get; set; }
    // window processes runnable on a record (e.g. Lead -> Prospect) via VAAPI RunProcessBySearchKey.
    [JsonPropertyName("actions")] public Dictionary<string, ActionSpec>? Actions { get; set; }

    [JsonIgnore] public FieldSpec KeyField => Fields.First(f => f.IsKey);
    public FieldSpec? FieldByApi(string api) =>
        Fields.FirstOrDefault(f => string.Equals(f.Api, api, StringComparison.OrdinalIgnoreCase));
}

public sealed class KeySpec
{
    [JsonPropertyName("api")] public string Api { get; set; } = "key";
    [JsonPropertyName("column")] public string Column { get; set; } = "Value";
    [JsonPropertyName("label")] public string? Label { get; set; }
}

public sealed class FieldSpec
{
    [JsonPropertyName("api")] public string Api { get; set; } = "";
    [JsonPropertyName("column")] public string Column { get; set; } = "";
    // string | boolean | number | ref | list | ownedRef | derived
    [JsonPropertyName("type")] public string Type { get; set; } = "string";
    [JsonPropertyName("required")] public bool Required { get; set; }
    [JsonPropertyName("isKey")] public bool IsKey { get; set; }
    [JsonPropertyName("maxLen")] public int? MaxLen { get; set; }
    [JsonPropertyName("default")] public JsonElement? Default { get; set; }
    [JsonPropertyName("ref")] public RefSpec? Ref { get; set; }
    [JsonPropertyName("list")] public Dictionary<string, string>? List { get; set; }
    // ownedRef: the value is a nested object inserted into owned.table first; its PK fills this column.
    [JsonPropertyName("owned")] public OwnedSpec? Owned { get; set; }
    // derived: looked up from another already-resolved field's row (no caller input).
    [JsonPropertyName("derived")] public DerivedSpec? Derived { get; set; }
    // alsoColumns: mirror this field's resolved value into extra columns (e.g. price -> PriceEntered/PriceActual/PriceList).
    [JsonPropertyName("alsoColumns")] public List<string>? AlsoColumns { get; set; }
    [JsonPropertyName("help")] public string? Help { get; set; }
}

/// <summary>
/// A value derived by looking up a column from a row keyed by another already-resolved field.
/// e.g. AD_Org_ID = SELECT AD_Org_ID FROM C_BPartner WHERE C_BPartner_ID = {resolved businessPartner}.
/// `from` is the api name of another field in the same object; "$parent" = the parent record's PK.
/// </summary>
public sealed class DerivedSpec
{
    [JsonPropertyName("from")] public string From { get; set; } = "";
    [JsonPropertyName("table")] public string Table { get; set; } = "";
    [JsonPropertyName("whereColumn")] public string WhereColumn { get; set; } = "";
    [JsonPropertyName("selectColumn")] public string SelectColumn { get; set; } = "";
    [JsonPropertyName("extraWhere")] public string? ExtraWhere { get; set; }
}

/// <summary>A 1:N child collection (e.g. BusinessPartner.contacts -> AD_User).</summary>
public sealed class ChildSpec
{
    [JsonPropertyName("table")] public string Table { get; set; } = "";
    [JsonPropertyName("pk")] public string Pk { get; set; } = "";
    [JsonPropertyName("parentLink")] public string ParentLink { get; set; } = ""; // FK column -> parent PK
    [JsonPropertyName("includeStandardCols")] public bool IncludeStandardCols { get; set; } = true;
    [JsonPropertyName("fields")] public List<FieldSpec> Fields { get; set; } = new();
    // optional hierarchical placement of the child's auto-created tree node (e.g. account under a parent account).
    [JsonPropertyName("treeNode")] public TreeNodeSpec? TreeNode { get; set; }
}

/// <summary>
/// Re-parents a tree-supported record's node after insert. The model auto-creates the node at the tree
/// root; if the caller supplies `parentField` (the parent record's identifier), the node is moved under it.
/// </summary>
public sealed class TreeNodeSpec
{
    [JsonPropertyName("nodeTable")] public string NodeTable { get; set; } = "";        // e.g. AD_TreeNode_EV
    [JsonPropertyName("nodeColumn")] public string NodeColumn { get; set; } = "Node_ID";
    [JsonPropertyName("parentColumn")] public string ParentColumn { get; set; } = "Parent_ID";
    [JsonPropertyName("treeColumn")] public string TreeColumn { get; set; } = "AD_Tree_ID";
    [JsonPropertyName("treeFrom")] public string TreeFrom { get; set; } = "";          // parent-entity field holding the tree id
    [JsonPropertyName("parentField")] public string ParentField { get; set; } = "parent"; // child payload key
    [JsonPropertyName("parentRef")] public RefSpec? ParentRef { get; set; }            // resolve parent identifier -> id
    [JsonPropertyName("scopeColumn")] public string? ScopeColumn { get; set; }         // scope parent lookup by this = parent PK
}

/// <summary>A window process runnable on a record (e.g. "Generate Prospect" on a Lead).</summary>
public sealed class ActionSpec
{
    [JsonPropertyName("searchKey")] public string? SearchKey { get; set; } // RunProcessBySearchKey
    [JsonPropertyName("processId")] public int? ProcessId { get; set; }    // alt: RunProcess by AD_Process_ID
    [JsonPropertyName("label")] public string? Label { get; set; }
    [JsonPropertyName("menu")] public bool Menu { get; set; }              // menu process -> Record_ID = 0
    [JsonPropertyName("params")] public List<ActionParamSpec>? Params { get; set; }
}

/// <summary>A parameter passed to a process. Value comes from the request body, a const, or a resolved ref.</summary>
public sealed class ActionParamSpec
{
    [JsonPropertyName("name")] public string Name { get; set; } = "";   // process parameter name
    [JsonPropertyName("api")] public string? Api { get; set; }          // request-body key (defaults to name)
    [JsonPropertyName("type")] public string Type { get; set; } = "string"; // string|number|boolean|ref|list
    [JsonPropertyName("required")] public bool Required { get; set; }
    [JsonPropertyName("const")] public JsonElement? Const { get; set; }  // always-sent constant
    [JsonPropertyName("ref")] public RefSpec? Ref { get; set; }
    [JsonPropertyName("list")] public Dictionary<string, string>? List { get; set; }
    [JsonPropertyName("help")] public string? Help { get; set; }
    [JsonIgnore] public string Key => Api ?? Name;
}

/// <summary>An owned sub-record inserted before its host (e.g. C_BPartner_Location.address -> C_Location).</summary>
public sealed class OwnedSpec
{
    [JsonPropertyName("table")] public string Table { get; set; } = "";
    [JsonPropertyName("pk")] public string Pk { get; set; } = "";
    [JsonPropertyName("includeStandardCols")] public bool IncludeStandardCols { get; set; } = true;
    [JsonPropertyName("fields")] public List<FieldSpec> Fields { get; set; } = new();
}

public sealed class RefSpec
{
    [JsonPropertyName("table")] public string Table { get; set; } = "";
    [JsonPropertyName("matchOn")] public string MatchOn { get; set; } = "Value";
    [JsonPropertyName("returns")] public string Returns { get; set; } = "";
}

// ---- Manifest loader -------------------------------------------------------

public sealed class ManifestStore
{
    private readonly Dictionary<string, EntityManifest> _byEntity = new(StringComparer.OrdinalIgnoreCase);

    public ManifestStore(IConfiguration cfg, ILogger<ManifestStore> log)
    {
        var dirName = cfg["SmartApi:ManifestDirectory"] ?? "manifests";
        // Resolve next to the app binaries (works for `dotnet run` bin/ and IIS publish alike).
        var dir = Path.IsPathRooted(dirName) ? dirName : Path.Combine(AppContext.BaseDirectory, dirName);
        if (!Directory.Exists(dir))
        {
            log.LogWarning("Manifest directory not found: {Dir}", dir);
            return;
        }

        var opts = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        foreach (var file in Directory.EnumerateFiles(dir, "*.json", SearchOption.AllDirectories))
        {
            try
            {
                var m = JsonSerializer.Deserialize<EntityManifest>(File.ReadAllText(file), opts);
                if (m is null || string.IsNullOrWhiteSpace(m.Entity)) continue;
                _byEntity[m.Entity] = m;
                log.LogInformation("Loaded manifest '{Entity}' -> {Table} ({Fields} fields)",
                    m.Entity, m.Table, m.Fields.Count);
            }
            catch (Exception ex)
            {
                log.LogError(ex, "Failed to load manifest {File}", file);
            }
        }
    }

    public bool TryGet(string entity, out EntityManifest manifest) => _byEntity.TryGetValue(entity, out manifest!);
    public IEnumerable<string> Entities => _byEntity.Keys;
}
