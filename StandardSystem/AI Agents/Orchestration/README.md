# Orchestration build — prompts & knowledge bases

Production-ready prompts and knowledge bases for the AURA → Dispatcher → Foundation orchestrations.
Routing reliability is the design priority: **defense-in-depth** — the Dispatcher re-checks AURA's intent
(catching how-to questions mis-sent as EXECUTE), and every Supervisor refuses to trap the user mid-flow.

## File → where it goes in the model

| File | Agent | `VAI01_OAgent` type | Field |
|---|---|---|---|
| `Prompts/Implementation_Dispatcher.md` | Implementation Dispatcher | SU | `VAI01_Prompt` |
| `Prompts/Foundation_Setup_Supervisor.md` | Foundation Setup Supervisor | SU | `VAI01_Prompt` |
| `Prompts/Accounting_Foundation_Supervisor.md` | Accounting Foundation Supervisor | SU | `VAI01_Prompt` |
| `Prompts/Acct_Foundation_Discovery.md` | Acct Foundation Discovery Agent | MO | `VAI01_Prompt` |
| `Prompts/Acct_Foundation_Advisor.md` | Acct Foundation Advisor (recommendation engine) | MO | `VAI01_Prompt` |
| `Prompts/Acct_Schema_Specialist.md` | Accounting Schema Specialist | AC | `VAI01_Prompt` |
| `Prompts/Chart_Of_Accounts_Specialist.md` | Chart of Accounts Specialist | AC | `VAI01_Prompt` |
| `Prompts/Account_Defaults_Specialist.md` | Account Defaults Specialist | AC | `VAI01_Prompt` |
| `Prompts/Account_Defaults_Execution_Subagent.md` | Account Defaults Execution Subagent | AC | `VAI01_Prompt` |
| `KnowledgeBase/Implementation_Dispatcher_KB.md` | Implementation Dispatcher | — | `VAI01_AgentKnowledgeBase` → `VAI01_AgentKBLine` |
| `KnowledgeBase/Accounting_Foundation_KB.md` | Acct Foundation Supervisor + Advisor | — | `VAI01_AgentKnowledgeBase` → `VAI01_AgentKBLine` |
| `KnowledgeBase/Platform_Foundation_KB.md` | Foundation Setup Supervisor | — | `VAI01_AgentKnowledgeBase` → `VAI01_AgentKBLine` |

Paste the fenced ```text block from each prompt file into `VAI01_Prompt`. Load each KB as KB lines (or as
the supervisor's `VAI01_CentralKnowledgebase` reference).

## Runtime placeholders (inject at call time)
`{datetime}` · `{user_id}` · `{user_roles}` · `{active_workflow}` · `{orchestration_registry}` ·
`{progress_state}` · `{client_id}` · `{org_id}`. These mirror the f-string style of the current AURA prompt.

## How the routing fix works (the biggest issue)
1. **AURA** decides the lane — the hard boundary is **READ vs EXECUTE**. (See the AURA prompt from the
   earlier design note; not re-included here.)
2. **Dispatcher** (this set) re-validates intent: if an EXECUTE command is really a how-to/data/document
   question, it **HANDBACKs** instead of forcing an orchestration — this is the second line of defense
   against the "configuration request → User Manual" mis-route.
3. **Dispatcher** then selects the orchestration via auth-filter → domain-narrow → trigger match →
   confidence gate (ROUTE / DISAMBIGUATE / NO_MATCH). It never falls back to the manual or guesses.
4. **Supervisors** own the conversation only while active, honor interrupts, and HANDBACK general
   questions rather than trapping the user — keeping routing correct *through* the whole conversation.

## Build & runtime standard (authoritative)
`Orchestration_Build_Standards.md` — distilled from the user's "Onfinity Orchestration Script Guidelines".
Governs script generation (tenant resolution, AD_Sequence PKs, audit cols, duplicate-prevention, live
metadata validation, LLM config, tool bindings + CustomParams templates, label limits) AND runtime behavior
(Supervisor §9 = orchestrate-don't-configure + request phases; Specialist §10 = consultant loop; show
business labels not internal IDs; readable formatting; transparent error handling). **All prompts below
conform to it.**

### Role-model alignment (per the standard)
- **Supervisor** = router + phase-manager + gate-enforcer + shared business-profile intake. Does NOT collect
  field values, recommend config, write, or expose IDs.
- **Specialist** = the implementation **consultant** that runs `collect→explain→validate→recommend→propose→
  confirm→execute→verify` and performs the write via tools. **The business-type recommendations live here**
  (the Advisor is the Specialists' recommendation engine).
- The agents earlier called "**Executor**" are these **Specialists** (with optional **Subagents** for
  detailed execution), matching the standard's hierarchy `Supervisor → Specialist → Subagent → Tools`.

## Built so far
- AURA-level: Implementation Dispatcher (+ routing KB).
- Platform Foundation: Supervisor (+ KB). Specialists pending.
- Accounting Foundation: Supervisor, Discovery, Advisor, **Specialists** (Schema, Chart of Accounts,
  Account Defaults) + **Account Defaults Execution Subagent**, KB (consultative + tools-per-task).
- `Orchestration_Build_Standards.md` (authoritative build + runtime standard).

## Not yet built (next steps)
- The **Platform Foundation Specialist** prompts (Organization / Currency / Calendar & Period / Document &
  Sequence / Roles & Access + its Execution Subagent) — each a §10 consultant.
- The `VAI01_OAgentDelegation` rows (Supervisor → Specialists; Specialist → Subagent) and `VAI01_OAgentAPI`
  tool bindings with `VAI01_CustomParams` JSON templates (Build Standards §B); read-only for SU/MO, write for AC.
- The **orchestration registry rows** + routing manifests that populate `{orchestration_registry}`.
- The import **SQL generator** following Build Standards §A (tenant-dynamic, AD_Sequence, duplicate-safe, Oracle).
