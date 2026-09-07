using OnfinityWorkflowHost;

var builder = WebApplication.CreateBuilder(args);

// the background workflow processor (timers / escalations / async continuation)
builder.Services.AddHostedService<WorkflowProcessorService>();

var app = builder.Build();

// wire the VA data layer to Postgres before serving
VaRuntime.InitDatabase(app.Configuration);

// liveness + DB check
app.MapGet("/health", () => Results.Ok(new
{
    status      = "ok",
    service     = "onfinity-workflow-host",
    dbConnected = VaRuntime.IsDbConnected()
}));

// start a (document/value) workflow — the handoff point for the web app, agents, integrations
app.MapPost("/workflow/start", (StartRequest req) =>
{
    try { return Results.Ok(VaRuntime.StartWorkflow(req)); }
    catch (Exception ex) { return Results.Problem(ex.Message); }
});

app.Run();
