# Implementation Dispatcher — Knowledge Base

> Maps to `VAI01_AgentKnowledgeBase` / `VAI01_AgentKBLine` for the Implementation Dispatcher.
> Purpose: make orchestration routing reliable. This is the reference the Dispatcher uses to match an
> EXECUTE command to the right orchestration and to avoid the mis-routing that happens today.

## 1. The routing problem, stated plainly
The recurring failure is **intent confusion**, not topic confusion. "Configure tax" and "how do I
configure tax" share the topic *tax* but have opposite intents:
- "how do I configure tax" = the user wants to **learn** → belongs to the User Manual lane (handled by AURA).
- "configure tax for India" = the user wants the system to **do it** → belongs to an orchestration.

The Dispatcher only receives commands AURA already tagged EXECUTE. Its job is (a) confirm they are truly
EXECUTE, and (b) pick the one right orchestration. When unsure, ask — never guess.

## 2. Intent test (apply before routing)
| Signal in the message | Intent | Action |
|---|---|---|
| Imperative, state-changing verb: create, configure, set up, define, register, assign, post, generate, enable, onboard | EXECUTE | route to an orchestration |
| Interrogative about procedure: how / where / what / can you explain | LEARN | HANDBACK (manual) |
| "show / list / what is the current value / report" | RETRIEVE | HANDBACK (data) |
| "in the document / contract / file I uploaded" | DOCS | HANDBACK (DMS) |
Politeness does not change intent: "could you set up the schema?" is still EXECUTE.

## 3. Orchestration registry — record format
Every orchestration the Dispatcher can route to is described by a manifest. Match against `purpose` +
`example_triggers`; rule out with `anti_triggers`; pre-filter by `domain` and `authorized_roles`.

```
name:            Accounting Foundation
value:           ACCT_FOUNDATION
domain:          Finance
purpose:         Set up the shared accounting backbone — accounting schema, chart of accounts, posting defaults.
example_triggers: ["set up accounting schema", "create the chart of accounts", "configure posting defaults",
                   "set up accounting for <org>", "enable accounting"]
anti_triggers:   ["how do I ...", "explain ...", "AP/AR/bank/tax configuration" (→ Finance Module)]
required_inputs: [organization, base currency, calendar]   # prerequisites
authorized_roles: [Implementation Consultant, System Admin]
status:          active
```

## 4. Domain taxonomy (controlled vocabulary for the narrow step)
`Finance · HRM · SCM · CRM · Assets · Service · Manufacturing · Pricing · Reporting · Platform · Communication · AI/Services`
Infer the command's domain, then prefer orchestrations tagged with it. (Mirrors the system's module taxonomy.)

## 5. Decision flow (summary)
1. Continuation: in a workflow? default-continue unless interrupt → else HANDBACK(INTERRUPT).
2. Intent sanity: really EXECUTE? if LEARN/RETRIEVE/DOCS → HANDBACK with that reason.
3. Auth filter: keep only orchestrations the user's roles allow.
4. Domain narrow: filter by domain tag.
5. Match: purpose + triggers; anti-triggers rule out.
6. Confidence gate: strong single → ROUTE; two close → DISAMBIGUATE; none → NO_MATCH (never fall back).

## 6. Worked examples (positive, negative, near-miss)
| User says | Decision | Target / reason |
|---|---|---|
| "Set up the accounting schema for Acme" | ROUTE | Accounting Foundation |
| "Configure organizations and base currency" | ROUTE | Foundation Setup |
| "Create GST-18 tax rate" | ROUTE | Finance Module (Tax) — or DISAMBIGUATE if a standalone Tax orch exists |
| "How do I create a tax rate?" | HANDBACK · LEARN | how-to → manual (the classic mis-route to avoid) |
| "What is a chart of accounts?" | HANDBACK · LEARN | definition → manual |
| "List GL accounts we created" | HANDBACK · RETRIEVE | data lookup |
| "Onboard new subsidiary" (2 onboarding orchestrations) | DISAMBIGUATE | name both, ask which |
| "Set up payroll" (user lacks the role) | NO_MATCH | state it needs access; name closest |
| [in Accounting Foundation] "use INR" | ROUTE | continuation → Accounting Foundation |
| [in Accounting Foundation] "what's costing method?" | HANDBACK · INTERRUPT/LEARN | answer, then resume |

## 7. Anti-patterns (do NOT do these)
- ❌ Falling back to the User Manual when no orchestration matches. → Instead: NO_MATCH + closest options.
- ❌ Forcing a how-to question into an orchestration because the topic matched. → Check intent first.
- ❌ Picking the "closest-looking" orchestration on weak evidence. → DISAMBIGUATE or NO_MATCH.
- ❌ Routing to an orchestration the user's role can't run. → Respect auth; say so.
- ❌ Trapping the user in an active workflow. → Honor interrupts; HANDBACK.

## 8. Registration note (for whoever adds orchestrations)
Each new orchestration must ship a manifest (purpose, example_triggers, anti_triggers, domain,
required_inputs, authorized_roles). Auto-draft it from the orchestration's Task/Description at creation,
then have the creator confirm. Poor triggers = poor routing — this is the single biggest lever on accuracy.
```
