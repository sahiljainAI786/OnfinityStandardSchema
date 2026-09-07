/********************************************************
 * net8 split — HEADLESS reporting seam (no-op implementations).
 *
 * On the Linux/headless engine, report RENDERING is not available: reports are routed to the
 * Windows report node per the deployment design (the GDI/Crystal renderers live in the separate
 * VAModelAD.Reporting assembly, net48). These no-op types let VAModelAD.Core compile and run
 * the process engine headless — any path that would actually render a report returns null/false
 * instead of producing output.
 *
 * A Windows (net48) build EXCLUDES this file and includes the real VAModelAD.Reporting types
 * instead (same namespaces, real GDI implementations) — i.e. this is the headless half of a
 * conditional seam, not a permanent replacement.
 ********************************************************/
using VAdvantage.Utility;
using VAdvantage.ProcessEngine;
using VAdvantage.DataBase;
using VAdvantage.Logging;

namespace VAdvantage.Print
{
    /// <summary>Opaque render artifact (a laid-out report view). Not produced headless.</summary>
    public class View { }

    /// <summary>Print-format model placeholder. The real MPrintFormat (AD_PrintFormat, GDI-tangled)
    /// lives in the Reporting assembly; headless only needs it as a type in IReportView signatures
    /// plus the few AD_* getters the process engine reads (return 0 — no print format headless).</summary>
    /// <summary>Headless print-format model: the real AD_PrintFormat data model (clean X_ base in
    /// XModel) so callers get the genuine getters/setters; only the GDI *rendering* is stubbed.</summary>
    public class MPrintFormat : VAdvantage.Model.X_AD_PrintFormat
    {
        public MPrintFormat(Ctx ctx, int AD_PrintFormat_ID, Trx trxName) : base(ctx, AD_PrintFormat_ID, trxName) { }
        public int TotalPage { get; set; }
    }

    /// <summary>Headless print-format-item model (real X_ base).</summary>
    public class MPrintFormatItem : VAdvantage.Model.X_AD_PrintFormatItem
    {
        public MPrintFormatItem(Ctx ctx, int AD_PrintFormatItem_ID, Trx trxName) : base(ctx, AD_PrintFormatItem_ID, trxName) { }
    }

    /// <summary>Headless stand-in for the static ReportCtl report launcher.</summary>
    public static class ReportCtl
    {
        public static IReportEngine Report { get; set; }
        public static IReportEngine Start(Ctx ctx, ProcessInfo pi, bool isDirectPrint) { return null; }
    }

    /// <summary>Shared no-op IReportEngine: every render entry point is a no-op headless.</summary>
    public abstract class HeadlessReportEngineBase : IReportEngine
    {
        public byte[] GetReportBytes() { return null; }
        public string GetReportString() { return null; }
        public string GetReportFilePath(bool fetchByteArr, out byte[] bytes) { bytes = null; return null; }
        public string GetCsvReportFilePath(string data) { return null; }
        public string GetRtfReportFilePath(string data) { return null; }
        public bool StartReport(Ctx ctx, ProcessInfo pi, Trx trx) { return false; }
    }

    /// <summary>Headless stand-in for the GDI ReportEngine_N renderer.</summary>
    public class ReportEngine_N : HeadlessReportEngineBase, IReportView
    {
        // document-type selectors (values are irrelevant headless — rendering is a no-op)
        public const int G = 0, ORDER = 1, INVOICE = 2, SHIPMENT = 3;
        public static ReportEngine_N Get(Ctx ctx, ProcessInfo pi) { return null; }
        public static ReportEngine_N Get(Ctx ctx, int docType, int recordId) { return null; }
        public View GetView() { return null; }
        public MPrintFormat GetPrintFormat() { return null; }
        public void SetPrintFormat(MPrintFormat pf) { }
        public byte[] CreateCSV(Ctx ctx) { return null; }
        public string GetRptHtml() { return ""; }
        public bool CreatePDF(string filePath) { return false; }
    }
}

namespace VAdvantage.ReportFormat
{
    /// <summary>Headless stand-in for the report-format engine factory.</summary>
    public class ReportFormatEngine
    {
        public static VAdvantage.Print.IReportEngine Get(Ctx ctx, ProcessInfo pi, bool isArabic) { return null; }
        public static VAdvantage.Print.IReportEngine Get(Ctx ctx, ProcessInfo pi, out int totalRecords, bool isArabic)
        {
            totalRecords = 0;
            return null;
        }
    }
}

namespace VAdvantage.CrystalReport
{
    /// <summary>Headless stand-in for the Crystal Reports renderer (real one is Windows-only).</summary>
    public class CrystalReportEngine : VAdvantage.Print.HeadlessReportEngineBase, VAdvantage.Print.IReportView
    {
        public CrystalReportEngine() { }
        public CrystalReportEngine(VAdvantage.Utility.Ctx ctx, VAdvantage.ProcessEngine.ProcessInfo pi) { }
        public VAdvantage.Print.View GetView() { return null; }
        public VAdvantage.Print.MPrintFormat GetPrintFormat() { return null; }
        public void SetPrintFormat(VAdvantage.Print.MPrintFormat pf) { }
    }
}

namespace VAdvantage.BiReport
{
    /// <summary>Headless stand-in for the BI report renderer.</summary>
    public class BiReportEngine : VAdvantage.Print.HeadlessReportEngineBase
    {
        public string GetReportString(VAdvantage.Utility.Ctx ctx, VAdvantage.Logging.VLogger log,
            VAdvantage.ProcessEngine.ProcessInfo pi) { return null; }
    }
}

namespace VAdvanatge.Report   // note: matches the (historically misspelled) namespace at the call sites
{
    /// <summary>Headless stand-in for the report-engine factory. Returns null (no renderer headless);
    /// callers already null-check the IReportEngine before rendering.</summary>
    public class ReportEngine
    {
        public static VAdvantage.Print.IReportEngine GetReportEngine(
            VAdvantage.Utility.Ctx ctx, VAdvantage.ProcessEngine.ProcessInfo pi,
            VAdvantage.DataBase.Trx trx, string assemblyName, string className)
        {
            return null;
        }
    }
}
