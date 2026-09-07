using VAdvantage.WF;   // note: VAdvantage.Utility is NOT imported — it has a 'Task' model that
                       // would clash with System.Threading.Tasks.Task; Ctx is qualified below.

namespace OnfinityWorkflowHost;

/// <summary>
/// The always-on background daemon: on a timer it enumerates the active workflow processors
/// (proving DB connectivity + engine liveness) and is the place where due workflow activities
/// (timers, escalations, sleeping/async steps) get advanced.
/// </summary>
public class WorkflowProcessorService : BackgroundService
{
    private readonly ILogger<WorkflowProcessorService> _log;
    private readonly TimeSpan _interval = TimeSpan.FromSeconds(30);

    public WorkflowProcessorService(ILogger<WorkflowProcessorService> log) => _log = log;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _log.LogInformation("Onfinity workflow processor started (interval {s}s)", _interval.TotalSeconds);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                if (VaRuntime.IsDbConnected())
                {
                    var ctx = new VAdvantage.Utility.Ctx();
                    var processors = MWorkflowProcessor.GetActive(ctx);
                    _log.LogInformation("WF heartbeat: {n} active workflow processor(s)", processors?.Length ?? 0);

                    // === RUNTIME WORK GOES HERE ===
                    // The activity-advancement loop — find suspended/sleeping AD_WF_Activity rows
                    // whose next-run time is due and resume them — lives in VA's server scheduler,
                    // NOT in VAWorkflow itself. To fully drive timers/escalations, port that loop
                    // (or query AD_WF_Activity directly and call the resume path). Document-triggered
                    // starts already run synchronously via the /workflow/start endpoint.
                }
                else
                {
                    _log.LogWarning("WF heartbeat: database not connected (check the Database config)");
                }
            }
            catch (Exception ex)
            {
                _log.LogError(ex, "WF processor loop error");
            }

            try { await Task.Delay(_interval, stoppingToken); }
            catch (TaskCanceledException) { break; }
        }

        _log.LogInformation("Onfinity workflow processor stopping");
    }
}
