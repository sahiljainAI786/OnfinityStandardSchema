using VAdvantage.DataBase;
using VAdvantage.Utility;
using VAdvantage.WF;
using VAdvantage.ProcessEngine;

namespace OnfinityWorkflowHost;

/// <summary>
/// Thin bootstrap around the ported VA workflow engine: wires the VA data layer to Postgres and
/// exposes the two entry points a headless host needs — health/liveness and "start a workflow".
/// </summary>
public static class VaRuntime
{
    /// <summary>Configure the VA global DB connection from appsettings ("Database" section).
    /// DBConn.SetPostgresConnectionString wires ALL the query paths: it sets the static connection
    /// string the SqlExec helpers (PostgreHelper) read, marks the connection Postgres, and sets the
    /// DB target — not just VConnection.s_cc (which only satisfies DB.IsConnected()).</summary>
    public static void InitDatabase(IConfiguration cfg)
    {
        var db   = cfg.GetSection("Database");
        var host = db["Host"] ?? "localhost";
        var port = db["Port"] ?? "5432";
        var name = db["Name"] ?? "onfinity";
        var user = db["User"] ?? "postgres";
        var pass = db["Password"] ?? "postgres";

        var npgsql = $"Server={host};Port={port};Database={name};User Id={user};Password={pass};";
        DBConn.SetPostgresConnectionString(npgsql);
    }

    public static bool IsDbConnected()
    {
        try { return DB.IsConnected(); }
        catch { return false; }
    }

    /// <summary>Build a minimal VA context for a request. NOTE: a production host must populate the
    /// full session context (client/org/role/user/language) — see the migration spec's "context
    /// reconstruction" item; the headless engine has no HttpSession to read it from.</summary>
    public static Ctx NewContext(int clientId, int userId)
    {
        var ctx = new Ctx();
        // minimal session context. A production host reconstructs the FULL context from the
        // caller (org/role/warehouse/language) — see the migration spec's "context reconstruction".
        ctx.SetContext("#AD_Client_ID", clientId.ToString());
        ctx.SetContext("#AD_Org_ID", "0");
        ctx.SetContext("#AD_User_ID", userId.ToString());
        ctx.SetContext("#AD_Role_ID", "0");
        ctx.SetContext("#AD_Language", "en_US");
        return ctx;
    }

    /// <summary>Start a workflow for a record (the API/queue handoff from the web app or an agent).
    /// Reproduces MWorkflow.Start's steps directly so any exception surfaces to the caller
    /// (MWorkflow.Start swallows it into VA's logger, which a headless host can't see).</summary>
    public static object StartWorkflow(StartRequest req)
    {
        var ctx = NewContext(req.AD_Client_ID, req.AD_User_ID);
        var wf  = new MWorkflow(ctx, req.AD_Workflow_ID, null);

        var pi = new ProcessInfo(wf.GetName(), 0, req.AD_Table_ID, req.Record_ID);
        pi.SetAD_Client_ID(req.AD_Client_ID);
        pi.SetAD_User_ID(req.AD_User_ID);

        var process = new MWFProcess(wf, pi);
        process.Save();
        process.StartWork();

        return new
        {
            started     = true,
            workflow    = wf.GetName(),
            wfProcessId = process.GetAD_WF_Process_ID(),
            wfState     = process.GetWFState()
        };
    }
}

/// <summary>Payload to start a (document/value) workflow.</summary>
public record StartRequest(int AD_Workflow_ID, int AD_Table_ID, int Record_ID, int AD_Client_ID, int AD_User_ID);
