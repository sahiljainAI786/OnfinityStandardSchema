using System.Text.Json;
using OnfinitySmartApi;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<ManifestStore>();
builder.Services.AddSingleton<ResolverCache>();
builder.Services.AddScoped<VaapiCredentials>();
builder.Services.AddScoped<UpsertService>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(o =>
{
    o.SwaggerDoc("v1", new() { Title = "Onfinity Smart API", Version = "v1",
        Description = "Identifier-based master-data wrapper over VAAPI. Send accessKey/secretKey headers (pass-through auth)." });
    // VAAPI key pass-through — two header API keys, both required.
    foreach (var key in new[] { "accessKey", "secretKey" })
        o.AddSecurityDefinition(key, new()
        {
            Name = key, Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
            In = Microsoft.OpenApi.Models.ParameterLocation.Header, Description = $"VAAPI {key}"
        });
    o.AddSecurityRequirement(new()
    {
        [new() { Reference = new() { Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme, Id = "accessKey" } }] = Array.Empty<string>(),
        [new() { Reference = new() { Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme, Id = "secretKey" } }] = Array.Empty<string>(),
    });
});

builder.Services.AddHttpClient<VaapiClient>((sp, http) =>
{
    var cfg = sp.GetRequiredService<IConfiguration>();
    http.BaseAddress = new Uri((cfg["Vaapi:BaseUrl"] ?? "https://localhost/api/VAAPI/Service").TrimEnd('/') + "/");
    http.Timeout = TimeSpan.FromSeconds(cfg.GetValue("Vaapi:TimeoutSeconds", 60));
});

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI(o => { o.SwaggerEndpoint("/swagger/v1/swagger.json", "Onfinity Smart API v1"); o.DocumentTitle = "Onfinity Smart API"; });

// translate our exception types into clean problem responses (must wrap endpoints)
app.Use(async (ctx, next) =>
{
    try { await next(); }
    catch (SmartApiException ex) { await Problem(ctx, ex.Status, ex.Code, ex.Message); }
    catch (VaapiException ex) { await Problem(ctx, 502, "VAAPI_ERROR", ex.Message); }
});

// --- auth: pull the caller's VAAPI key pair into the request scope ----------
app.Use(async (ctx, next) =>
{
    if (ctx.Request.Path.StartsWithSegments("/health") ||
        ctx.Request.Path.StartsWithSegments("/swagger")) { await next(); return; }

    var cred = ctx.RequestServices.GetRequiredService<VaapiCredentials>();
    cred.AccessKey = ctx.Request.Headers["accessKey"].FirstOrDefault();
    cred.SecretKey = ctx.Request.Headers["secretKey"].FirstOrDefault();
    if (!cred.IsPresent)
    {
        await Problem(ctx, 401, "NO_CREDENTIALS", "accessKey and secretKey headers are required.");
        return;
    }
    await next();
});

app.MapGet("/health", () => Results.Ok(new { status = "ok" }));
app.MapGet("/v1/_entities", (ManifestStore store) => Results.Ok(store.Entities));

// PUT = upsert · POST = insert-only
app.MapMethods("/v1/{entity}", new[] { "PUT", "POST" }, async (
    string entity, HttpContext ctx, ManifestStore store, UpsertService svc, CancellationToken ct) =>
{
    if (!store.TryGet(entity, out var m)) return NotFoundEntity(entity);
    var payload = await ReadBody(ctx, ct);
    var mode = HttpMethods.IsPost(ctx.Request.Method) ? WriteMode.InsertOnly : WriteMode.Upsert;
    var result = await svc.WriteAsync(m, payload, mode, keyFromPath: null, IsDryRun(ctx), ct);
    return Results.Ok(result);
});

// PATCH = update-only by key
app.MapMethods("/v1/{entity}/{key}", new[] { "PATCH" }, async (
    string entity, string key, HttpContext ctx, ManifestStore store, UpsertService svc, CancellationToken ct) =>
{
    if (!store.TryGet(entity, out var m)) return NotFoundEntity(entity);
    var payload = await ReadBody(ctx, ct);
    var result = await svc.WriteAsync(m, payload, WriteMode.UpdateOnly, keyFromPath: key, IsDryRun(ctx), ct);
    return Results.Ok(result);
});

// LIST/search — GET /v1/{entity}?field=value&...&page=&pageSize=&ids=true
// filters use the same field names as writes (identifiers in, identifiers out).
app.MapGet("/v1/{entity}", async (
    string entity, HttpContext ctx, ManifestStore store, UpsertService svc, CancellationToken ct) =>
{
    if (!store.TryGet(entity, out var m)) return NotFoundEntity(entity);
    var page = Math.Max(1, QueryInt(ctx, "page", 1));
    var pageSize = Math.Clamp(QueryInt(ctx, "pageSize", 50), 1, 200);
    var ids = QueryBool(ctx, "ids");
    var sort = ctx.Request.Query["sort"].ToString();
    var select = ctx.Request.Query["fields"].ToString();
    var reserved = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "page", "pageSize", "ids", "include", "sort", "fields" };
    var filters = ctx.Request.Query.Where(q => !reserved.Contains(q.Key))
                     .ToDictionary(q => q.Key, q => q.Value.ToString());
    var result = await svc.ListAsync(m, filters, page, pageSize, ids, sort, select, ct);
    return Results.Ok(result);
});

// GET one — FKs/lists as identifiers; ?include=children to embed child collections; ?ids=true to add raw FK ids
app.MapGet("/v1/{entity}/{key}", async (
    string entity, string key, HttpContext ctx, ManifestStore store, UpsertService svc, CancellationToken ct) =>
{
    if (!store.TryGet(entity, out var m)) return NotFoundEntity(entity);
    var dto = await svc.ReadAsync(m, key, QueryFlag(ctx, "include", "children"), QueryBool(ctx, "ids"),
                                 ctx.Request.Query["fields"].ToString(), ct);
    return dto is null ? NotFoundKey(m.Entity, key) : Results.Ok(dto);
});

// list the window processes (actions) available on an entity
app.MapGet("/v1/{entity}/_actions", (string entity, ManifestStore store) =>
{
    if (!store.TryGet(entity, out var m)) return NotFoundEntity(entity);
    var list = (m.Actions ?? new()).Select(kv => new
    {
        action = kv.Key,
        label = kv.Value.Label ?? kv.Key,
        searchKey = kv.Value.SearchKey,
        @params = (kv.Value.Params ?? new()).Select(p => new { name = p.Key, type = p.Type, required = p.Required, help = p.Help })
    });
    return Results.Ok(list);
});

// run a window process on a record: POST /v1/{entity}/{key}/actions/{action}
app.MapPost("/v1/{entity}/{key}/actions/{action}", async (
    string entity, string key, string action, HttpContext ctx, ManifestStore store, UpsertService svc, CancellationToken ct) =>
{
    if (!store.TryGet(entity, out var m)) return NotFoundEntity(entity);
    var body = await ReadBody(ctx, ct);
    var result = await svc.RunActionAsync(m, key, action, body, ct);
    return Results.Ok(result);
});

// run a MENU process (not bound to a record, Record_ID=0): POST /v1/{entity}/actions/{action}
app.MapPost("/v1/{entity}/actions/{action}", async (
    string entity, string action, HttpContext ctx, ManifestStore store, UpsertService svc, CancellationToken ct) =>
{
    if (!store.TryGet(entity, out var m)) return NotFoundEntity(entity);
    var body = await ReadBody(ctx, ct);
    var result = await svc.RunActionAsync(m, key: "", action, body, ct);
    return Results.Ok(result);
});

app.Run();

// ---- helpers ---------------------------------------------------------------

static int QueryInt(HttpContext ctx, string k, int def) =>
    ctx.Request.Query.TryGetValue(k, out var v) && int.TryParse(v, out var n) ? n : def;

static bool QueryBool(HttpContext ctx, string k)
{
    if (!ctx.Request.Query.TryGetValue(k, out var v)) return false;
    var s = v.ToString();
    return s is "" or "true" or "1";
}

static bool QueryFlag(HttpContext ctx, string k, string val) =>
    ctx.Request.Query.TryGetValue(k, out var v) &&
    v.ToString().Split(',', StringSplitOptions.TrimEntries).Contains(val, StringComparer.OrdinalIgnoreCase);

static bool IsDryRun(HttpContext ctx) =>
    ctx.Request.Query.TryGetValue("dryRun", out var v) &&
    (v == "true" || v == "1" || v.Count == 0 || string.IsNullOrEmpty(v));

static async Task<Dictionary<string, JsonElement>> ReadBody(HttpContext ctx, CancellationToken ct)
{
    if (ctx.Request.ContentLength is null or 0) return new();
    try { return await ctx.Request.ReadFromJsonAsync<Dictionary<string, JsonElement>>(ct) ?? new(); }
    catch (JsonException) { return new(); }
}

static IResult NotFoundEntity(string entity) =>
    Results.Json(new { error = new { code = "UNKNOWN_ENTITY", message = $"No manifest registered for '{entity}'." } }, statusCode: 404);

static IResult NotFoundKey(string entity, string key) =>
    Results.Json(new { error = new { code = "NOT_FOUND", message = $"{entity} '{key}' not found." } }, statusCode: 404);

static async Task Problem(HttpContext ctx, int status, string code, string message)
{
    ctx.Response.StatusCode = status;
    ctx.Response.ContentType = "application/json";
    await ctx.Response.WriteAsJsonAsync(new { error = new { code, message } });
}

