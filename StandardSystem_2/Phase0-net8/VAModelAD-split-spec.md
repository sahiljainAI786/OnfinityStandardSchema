# VAModelAD net8.0 Split — Implementation Spec

**Status:** plan for review (no code changed). Grounded in a file-level scan of
`StandardCode/Official-VAFramework/VAModelAD` (306 .cs files, ~110K LOC).
**Goal:** make the VAModelAD *core* compile and run headless on .NET 8 / Linux, isolating the
Windows-only reporting/printing code behind a seam — so VAWorkflow (which depends on VAModelAD)
can become a net8 assembly.

---

## 0. Correction to the earlier closure assessment (honesty note)

The earlier closure trace listed **IKVM/Java as a dead reference to drop**. That was wrong — it
came from a grep bug (`\|` is a *literal* pipe in `-E` mode, so the search matched nothing).
A corrected scan shows **IKVM/Java is actively used in ~7 core files (40 call sites)**. It is a
real migration workstream (WS3 below), not a deletion. Everything else in the closure assessment
holds.

---

## 1. Target topology

Split today's single `VAModelAD.dll` into:

| New assembly | TFM | Contents | Notes |
|---|---|---|---|
| **VAModelAD.Core** | `net8.0` (+`net48` on build server) | ORM/PO, ProcessAD, ProcessEngine, Model, Lookup, Controller, Utility, AIHelper, Common, etc. | the headless engine dependency |
| **VAModelAD.Reporting** | `net48` (Windows) **or** replace | `Print/` (36), `CrystalReport/` (3), `Report/`, `MPrintPaper.cs`, the concrete `ReportEngine` | stays on Windows print node; or rewrite on a cross-platform engine later |
| **(seam)** | in Core | `IReportEngine` interface + registration | Core depends on the interface, never the GDI impl |

Core depends on already-ported **CoreLibrary, BaseLibrary, XModel** (all green in `Phase0-net8/`).

---

## 2. Five workstreams

### WS1 — Clean folder moves → VAModelAD.Reporting
Move wholesale (these are the GDI/Crystal tier):
- `Print/` — 36 files (incl. 11 `System.Drawing.Printing`, the concrete `ReportEngine`/`ReportEngine_N`)
- `CrystalReport/` — 3 files (Crystal Reports, Windows-only)
- `Report/ReportFormatEngine.cs` — wraps `ReportEngine` (10 refs)
- `ModelAD/MPrintPaper.cs` — `System.Drawing.Printing` paper config

No interface needed for these — they ARE the reporting impl. They keep `net48` (or get rewritten
on QuestPDF/SkiaSharp if/when you want Linux-side rendering).

### WS2 — The `ReportEngine` seam (the architectural core) ⭐
The core process path calls the GDI `ReportEngine` ~58 times across **6 files that must stay in Core**:

| File | ReportEngine refs | Role |
|---|---|---|
| `ModelAD/MScheduler.cs` | 23 (+2 `_N`) | scheduled processes that emit reports |
| `ProcessEngine/ProcessCtl.cs` | 19 (+4 `_N`) | process controller |
| `Common/Common.cs` | 12 (+2 `ReportFormatEngine`) | shared helpers |
| `ProcessEngine/SvrProcess.cs` | 1 | server-process base |
| `Classes/ModelFactory.cs` | 1 (`VAdvantage.Print.M…`) | model factory |

**Plan:**
1. Define `IReportEngine` (and `IReportFormatEngine`) in Core (`Interface/` or `Common/`) exposing
   only the methods these call sites use (generate-to-PDF, generate-and-email, run-for-process…).
2. Replace `new ReportEngine(...)` / static calls in the 6 files with calls through the injected
   `IReportEngine` (resolved via the existing `Ctx`/service-locator, or a static
   `ReportEngineProvider.Current` set at host startup).
3. The concrete `ReportEngine` (in Reporting) implements `IReportEngine`. On the **headless Linux
   engine**, register a **no-op / "reporting-not-available-here"** implementation (workflows that
   don't render reports are unaffected; ones that do are routed to the Windows report node — matches
   the deployment design already in memory).

This is the one genuinely architectural piece. Bounded: ~58 call sites, 6 files, one interface.

### WS3 — De-Java (remove IKVM dependency from Core)
40 `java.*` call sites in 7 core files (IKVM.OpenJDK is .NET-Framework-only):

| File | sites | java APIs used | .NET replacement |
|---|---|---|---|
| `ModelAD/MSchedule.cs` | 19 | `java.util.Calendar`, `java.util.regex` | `System.DateTime`/`Calendar`, `System.Text.RegularExpressions` |
| `ModelAD/RModel.cs` | 6 | regex / calendar | same |
| `ModelAD/MMailAttachment1.cs` | 3 | `java.io.FileInputStream` | `System.IO.FileStream` |
| `ModelAD/MAttachment.cs` | 3 | file / zip | `System.IO`, `System.IO.Compression` |
| `Utility/EMail.cs` | 2 | regex | `System.Text.RegularExpressions` |
| `Controller/GridTable.cs` | 2 | `java.awt.Dimension` | `System.Drawing.Size` or a small struct |
| `ProcessEngine/MPInstance.cs` | 1 | misc | .NET equiv |

**Plan:** replace java.* with BCL equivalents (the better long-term fix; removes IKVM entirely).
`MSchedule.cs` is the bulk (recurrence calc on `java.util.Calendar` → `DateTime`). Alternative
(faster, lower-quality): reference modern **IKVM 8.x** (net8-capable) — but that drags a JVM-on-.NET
runtime into the headless engine; **de-Java is recommended**.

### WS4 — WinForms tree model
`ModelAD/MTree.cs` (+ `MTreeNode*`) use `VTreeNode` (which extends WinForms `TreeNode` — already
excluded from BaseLibrary). Only **5 core files use MessageBox** (trivial → `ILogger`/exception).

**Plan:** the tree model is navigation/menu UI data. Either (a) relocate `MTree`/tree classes to the
UI layer (if the headless engine never builds menu trees — likely), or (b) replace `VTreeNode`'s
WinForms base with a plain `VTreeNodeBase { Text; List<VTreeNode> Nodes; }` (a ~1-file shim reused by
both BaseLibrary and here). Decide per whether workflows touch the tree model (probably not).

### WS5 — Dead-ref purge, package retargets, dedup
**Genuinely dead in VAModelAD (drop from csproj):** `Interop.Excel`, `Antlr3.Runtime`,
`Google.Apis`, `Mono.Security`, `WorkspaceSvc/WSP`.
**Used → retarget to net8 packages:** Oracle (5 files → `Oracle.ManagedDataAccess.Core`),
`Renci.SshNet` (1 → net8 build), `ICSharpCode.SharpZipLib` (1 → net8 build), Npgsql/MySql/Newtonsoft/
Konscious/Microsoft.Bcl/Extensions (as in CoreLibrary). Add `System.Data.SqlClient`,
`System.Drawing.Common` (core image use: ~16 real `System.Drawing.Image`/`Bitmap` sites — compiles;
**Linux-runtime caveat** if those paths run headless).
**VAI01** (3 files, AI-orchestration internal) — port or isolate; check separately.
**Dedup:** delete the duplicate `ModelAD/MSysConfig.cs` (identical to ModelLibrary's; keep one).

---

## 3. Build & validation order
1. WS5 dead-ref purge + package retargets (mechanical, no logic change).
2. WS1 move Print/CrystalReport/Report/MPrintPaper out to `VAModelAD.Reporting` (net48).
3. WS2 introduce `IReportEngine`, rewire the 6 seam files. **← review checkpoint**
4. WS3 de-Java the 7 files (start with MSchedule).
5. WS4 tree-model decision.
6. `dotnet build` VAModelAD.Core for net8.0 → iterate to 0 errors (expect the now-familiar buckets).
7. Then XModel already green → build **VAWorkflow** against Core → first net8 engine assembly.

---

## 4. Risks & open decisions (need your input)
- **D1 — Reporting target:** keep `VAModelAD.Reporting` on net48/Windows (fast, hybrid per the
  deployment plan) **or** invest now in a cross-platform rewrite (QuestPDF/SkiaSharp)? Recommend
  net48-for-now; the headless engine uses the no-op `IReportEngine`.
- **D2 — IKVM:** de-Java (recommended, clean) vs reference modern IKVM 8.x (faster, drags JVM). 40 sites.
- **D3 — `System.Drawing.Common` on Linux:** ~16 core image sites compile but throw on Linux at
  runtime. Acceptable if those paths (attachment image processing, logos) don't run in the headless
  engine; otherwise → ImageSharp/SkiaSharp. Needs a runtime-path audit.
- **D4 — Tree model:** confirm the headless engine never builds menu/nav trees (then relocate MTree
  to UI). If it does, do the `VTreeNodeBase` shim.

## 5. Rough effort
- WS5 (purge/retarget/dedup): ~0.5 day, mechanical.
- WS1 (folder moves + Reporting csproj): ~0.5 day.
- WS2 (IReportEngine seam, 58 sites/6 files): ~1–2 days — the real work.
- WS3 (de-Java, 40 sites): ~1 day (MSchedule recurrence is the tricky bit).
- WS4 (tree decision): ~0.5 day.
- Integration + net8 build-fix iterations: ~1 day.
- **Total ≈ 4–6 focused days** to a green VAModelAD.Core, then VAWorkflow links.
