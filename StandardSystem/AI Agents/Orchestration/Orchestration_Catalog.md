# Onfinity ERP Implementation — Orchestration Catalog

> The full set of **implementation** orchestrations across all modules, in the layered structure:
> Program (T0) → Global Foundations (T1) → Module Implementations (T2) → Lifecycle/Cross-cutting (T3).
> Each row seeds an orchestration registry record (`VAI01_AIOrchestration`): Name + `Value` (search key) +
> Domain tag + scope + prerequisites. Module codes (VA0xx) map to the live module taxonomy.
> Rule applied: one orchestration per module; shared prerequisites pulled up; cross-cutting sub-areas
> (Tax, Master Data) promoted to their own; large modules may split later.

## Tier 0 — Program
| Orchestration | Value | Domain | Scope |
|---|---|---|---|
| Implementation Program Orchestrator | `PROG_IMPL` | Platform | Sequences the whole rollout, holds the shared business profile, enforces prerequisites, tracks progress, delegates to all below. |

## Tier 1 — Global Foundations (prerequisites for everything)
| Orchestration | Value | Domain | Scope | Prereq |
|---|---|---|---|---|
| Platform Foundation | `CORE_FOUNDATION` | Platform | Org, currency, calendar & periods, document types & number sequences, roles & access. | Client |
| Accounting Foundation | `ACCT_FOUNDATION` | Finance | Accounting schema, chart of accounts, posting defaults (incl. posting rules / VA116). | Platform Foundation |
| Tax Configuration | `TAX_CONFIG` | Finance | Tax categories, rates, tax types (VATAX) — used by Finance, SCM, Sales. | Accounting Foundation |
| Master Data Foundation | `MASTER_DATA` | Platform | Business-partner groups, product categories, UoM, base price list, charges. | Platform (+ Accounting for accounting tabs) |

## Tier 2 — Module Implementations

### Finance & Treasury
| Orchestration | Value | Domain | Scope | Prereq |
|---|---|---|---|---|
| Finance Core (GL / AP / AR) | `FIN_CORE` | Finance | GL categories & journals, AP, AR, payment terms, GL voucher (VA028). | Accounting, Master Data, Tax |
| Banking & Cash | `FIN_BANK` | Finance | Banks, bank accounts, cash books, reconciliation setup. | Accounting |
| Payment Management | `FIN_PAYMENT` | Finance | Payment methods & payment-run setup (VA009). | Finance Core, Banking |
| Bank Statement Classification | `FIN_BANKSTMT` | Finance | Statement import & auto-classification rules (VA012). | Banking |
| Post-Dated Cheques | `FIN_PDC` | Finance | PDC handling & lifecycle (VA027). | Banking |
| Letter of Credit | `FIN_LOC` | Finance | LC setup & trade-finance documents (VA026). | Finance Core, Banking |
| Fixed Asset Management | `FIN_FIXEDASSET` | Assets | Asset categories, depreciation methods (VAFAM). | Accounting |
| Lease Asset Management | `FIN_LEASE` | Assets | Lease contracts & schedules (VA080). | Accounting |
| Amortization | `FIN_AMORT` | Finance | Amortization schedules (VA038). | Accounting |
| Budgeting & Costing | `FIN_BUDGET` | Finance | Budgets, cost centers, advance budget & costing (VA094, VA073). | Accounting |
| Financial Reporting | `FIN_REPORTS` | Reporting | Report column/line sets, financial reports, report painter (VA056, FRPT). | Accounting |

### Supply Chain & Operations
| Orchestration | Value | Domain | Scope | Prereq |
|---|---|---|---|---|
| Procurement / Purchasing | `SCM_PROCURE` | SCM | Vendors, purchase document types, purchase approval. | Master Data, Tax, Accounting |
| Inventory & Warehouse | `SCM_INVENTORY` | SCM | Warehouses, locators, inventory rules (Material Mgmt / DTD001). | Master Data, Accounting |
| Material Requirement Planning | `SCM_MRP` | SCM | Planning rules & parameters (VAMRP). | Inventory, Manufacturing |
| Material Quality Control | `SCM_QC` | SCM | QC plans & inspection (VA010). | Inventory |
| Manufacturing | `MFG_CORE` | Manufacturing | BOM, routing, work orders (VAMFG). | Inventory, Master Data |

### Sales, CRM & Pricing
| Orchestration | Value | Domain | Scope | Prereq |
|---|---|---|---|---|
| Sales & Order Management | `SALES_ORDER` | CRM | Sales document types, order rules, fulfillment. | Master Data, Tax, Pricing, Accounting |
| Pricing | `PRICING_CORE` | Pricing | Price lists, discount schemas (VAPRC, ED011). | Master Data |
| CRM | `CRM_CORE` | CRM | Leads, opportunities, activities, teams (VA061). | Master Data |
| Marketing | `MARKETING_CORE` | CRM | Campaigns & marketing setup (Market). | CRM |

### Human Resources
| Orchestration | Value | Domain | Scope | Prereq |
|---|---|---|---|---|
| HR Foundation | `HR_FOUNDATION` | HRM | Employee master, org structure, positions, pay elements (VA058). | Platform |
| Payroll | `HR_PAYROLL` | HRM | Pay structures, payroll runs (VA064). | HR Foundation, Accounting |
| Attendance Management | `HR_ATTENDANCE` | HRM | Shifts, attendance rules (VA069). | HR Foundation |
| Leave / Absence Management | `HR_ABSENCE` | HRM | Leave types & policies (VA086). | HR Foundation |
| Recruitment | `HR_RECRUIT` | HRM | Requisitions & hiring pipelines (VA062). | HR Foundation |
| Performance Management | `HR_PERFORMANCE` | HRM | Appraisal cycles & KPIs (VA065). | HR Foundation |
| Claims / Expense Management | `HR_CLAIMS` | HRM | Claim types & approval (VA072). | HR Foundation, Accounting |
| Employee Self-Service | `HR_ESS` | HRM | Self-service portal config (VA078). | HR Foundation |

### Service
| Orchestration | Value | Domain | Scope | Prereq |
|---|---|---|---|---|
| Service & Maintenance | `SERVICE_CORE` | Service | Equipment master, work orders, schedules, checklists (VA075). | Master Data, Inventory |

### Platform & Enablement
| Orchestration | Value | Domain | Scope | Prereq |
|---|---|---|---|---|
| Document Management | `DMS_CORE` | Platform | Document types, folders, RAG sources (VADMS). | Platform |
| Communication / Telephony | `COMM_CORE` | Communication | Call providers, connections, settings (VA048). | Platform |
| Reporting & Analytics | `REPORTING_CORE` | Reporting | Report engine, dashboards (VARPT). | Relevant modules |
| Workflow & Approvals | `WORKFLOW_CORE` | Platform | Approval workflows & document routing. | Platform |

## Tier 3 — Lifecycle / Cross-cutting (reusable across all modules)
| Orchestration | Value | Domain | Scope |
|---|---|---|---|
| Discovery & Blueprint | `LC_DISCOVERY` | Platform | Requirements gathering → configuration plan + business profile. |
| Data Migration | `LC_MIGRATION` | Platform | Master + transactional data import & reconciliation. |
| Validation & Readiness | `LC_VALIDATION` | Platform | Pre-go-live readiness checks across modules. |
| Go-Live / Cutover | `LC_GOLIVE` | Platform | Cutover sequencing & activation. |
| Audit & Compliance | `LC_AUDIT` | Platform | Compliance validation & audit trail review. |
| Recovery / Rollback | `LC_RECOVERY` | Platform | Remediation & safe rollback. |

## High-level sequence
```
LC_DISCOVERY → PROG_IMPL drives:
  CORE_FOUNDATION → ACCT_FOUNDATION → TAX_CONFIG → MASTER_DATA
     → (Finance · SCM · Sales/CRM · HR · Service · Platform modules, per their prerequisites)
        → LC_MIGRATION → LC_VALIDATION → LC_GOLIVE → LC_AUDIT   (LC_RECOVERY on demand)
```

## Notes
- ~45 orchestrations total. Each follows the same shape: Supervisor (SU) + Discovery (MO) + Specialists
  (AC, consultants) [+ Execution Subagents for high-risk writes] + Validation (QC) + Advisor (KB).
- This catalog seeds the dispatcher's orchestration registry — each row needs its routing manifest
  (purpose, example triggers, anti-triggers, required inputs, authorized roles) per the Dispatcher KB.
- Split later where a module outgrows one supervisor/specialist set; promote a sub-area to its own
  orchestration when it's reused across modules or independently requested.
