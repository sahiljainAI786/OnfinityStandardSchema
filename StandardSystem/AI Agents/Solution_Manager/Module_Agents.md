# Onfinity Solution Manager — Per-Module Agent Catalog (sequenced & load-balanced)

> Agents for the Solution Manager UI. Each **module** = a list of agents the user talks to **in the order
> shown** (implementation sequence). Every module opens with a **Solution Agent** (planner — recommends the
> order, drives "Run next step") and a **Readiness Check Agent** (prerequisite scan), then **one focused
> agent per setting area**.
>
> Design rules applied here:
> - **Setup only.** Agents configure *types, rules, masters, defaults* — they do NOT run operations
>   (no payroll runs, production execution, stock counts, leave requests). Those are runtime, out of scope.
> - **No overloaded agent.** Each agent owns a small, coherent set of setting areas. Big areas were split
>   (HR → its own module tiles; Item Master → category/attribute + product + costing; Tax → tax + withholding).
> - **Completeness.** Agents are mapped to the actual setup windows in the metadata (coverage notes per module).
> - All agents follow `Orchestration_Build_Standards.md` (tenant-dynamic IDs, AD_Sequence PKs, VAAPI tools,
>   show business labels/store IDs, confirm-before-write; high-impact agents are gated).

Sequence legend: **0** = Solution, **R** = Readiness, then ordered task agents.

---

## 1. Basic Setup  *(platform foundation — everything depends on it)*
| # | Agent | Setting areas it configures |
|---|---|---|
| 0 | Setup Solution Agent | plans the basic setup, recommends order, tracks progress |
| R | Readiness Check Agent | client/tenant exists, licensed modules, admin access |
| 1 | Company & Tenant Profile Agent | Tenant, legal entity, address, tax identifiers |
| 2 | Organization Structure Agent | Organization, Org Type, Org Group, Org Unit, Org Function |
| 3 | Localization Agent | Country, Region, Language, Tax Region, Sales Region |
| 4 | Currency & Exchange Rate Agent | Currency, Currency Conversion Source, Rate Type, Exchange Rate |
| 5 | Unit of Measure Agent | Unit of Measure + UoM conversions |
| 6 | Fiscal Calendar & Periods Agent | Calendar, Year, Period Control, Period Group, Holiday Calendar |
| 7 | Document Types & Sequences Agent | Document Type, Master/Posting Document Type, Transaction Sequence (numbering) |
| 8 | Roles & Users Agent | Role, Role Data View, Users, access groups |
> Covers the 29 BASIC windows. Sequence: identity → structure → localization → money/units → time → numbering → access.

## 2. Finance  *(Basic owns currency/calendar/numbering/roles)*
| # | Agent | Setting areas |
|---|---|---|
| 0 | Finance Solution Agent | plans & sequences Finance setup |
| R | Readiness Check Agent | org/currency/calendar present, ledger, finance role |
| 1 | Account Groups Agent | Account Group / element structure |
| 2 | Chart of Accounts Agent | natural accounts, control accounts, hierarchy/rollups |
| 3 | Accounting Dimensions Agent | Data Dimensions, Profit/Balance Dimension |
| 4 | Accounting Schema Agent | Accounting Book, costing method/level, GAAP, Accounting Parameters |
| 5 | Posting Defaults Agent ⚠ | Accounting Default (the default posting accounts) — *high-impact, gated* |
| 6 | GL & Charges Agent | GL Journal categories/batches setup, Charge Master |
| 7 | Tax Setup Agent | Tax Category, Tax Class, Tax Rate, Tax Setup, Tax Exemption Reason |
| 8 | Withholding & Income Tax Agent | Withholding Category, Withholding Tax, Income Tax Rate |
| 9 | Payment Terms & Dunning Agent | Payment Term, Dunning (collections setup) |
| 10 | Bank & Cash Agent | Bank, Bank Account, Cashbook, Cash Journal setup, payment methods |
| 11 | Budgeting Setup Agent *(optional)* | Budget, Budget Planning, Budgeting Key Figures/Parameters |
| 12 | Financial Reporting Agent | Report Account Type, report column/line sets, financial statements |
> Covers the 47 FINANCE windows. Tax split into Tax + Withholding; CoA split into Account Groups + accounts; Dimensions and Charges separated so no agent is overloaded.

## 3. Sales & CRM
| # | Agent | Setting areas |
|---|---|---|
| 0 | CRM Solution Agent | plans Sales & CRM setup |
| R | Readiness Check Agent | master data, pricing, accounting present |
| 1 | Customer Master Agent | customer business partners, customer groups, credit terms |
| 2 | Sales Territory & Team Agent | Sales Region, Team, Team Forecast |
| 3 | Lead Setup Agent | Lead Source, Lead Qualification, Lead Next Step (stages) |
| 4 | Opportunity & Pipeline Agent | Opportunity Type, Opportunity Source, pipeline stages |
| 5 | Price List & Discounts Agent | Price List, Discount Calculation/schemas |
| 6 | Quotation & Sales Document Agent | Sales Quotation templates, sales order/invoice document types |
| 7 | Commission Agent *(optional)* | Commission Setup, Commission Calculation |
| 8 | Marketing Campaign Agent *(optional)* | Marketing Campaign, Campaign Type, Marketing Channel, Target List |
> Covers the 27 SALES/CRM windows. Lead and Opportunity split (each has multiple config windows).

## 4. Procurement
| # | Agent | Setting areas |
|---|---|---|
| 0 | Procurement Solution Agent | plans Procurement setup |
| R | Readiness Check Agent | vendor master, tax, accounting present |
| 1 | Vendor Master Agent | Vendor Master, Vendor Details, vendor groups |
| 2 | Purchase Pricing & Terms Agent | purchase price lists, payment terms |
| 3 | Requisition Setup Agent | Requisition types & rules |
| 4 | Purchase Document Agent | Purchase Order, Blanket PO, RFQ/Tender document types & matching rules |
| 5 | Approval Matrix Agent ⚠ | approval limits, approver roles, routing (Workflow Approvals) — *gated* |
> Covers the 20 PROCURE windows (RFQ/Tender folded into Purchase Document setup).

## 5. Inventory  *(Item Master split to avoid overload)*
| # | Agent | Setting areas |
|---|---|---|
| 0 | Inventory Solution Agent | plans Inventory setup |
| R | Readiness Check Agent | accounting, product categories present |
| 1 | Warehouse & Locator Agent | Warehouse Structure, Locator, Warehouse Mgmt Rule/Strategy |
| 2 | Product Category & Attribute Agent | Product Category, Attribute, Attribute Group/Combination |
| 3 | Product Master Agent | Products/items (core master records) |
| 4 | Inventory Costing & Valuation Agent | costing method, Product Cost Element, Posting Category, valuation |
| 5 | Lot & Serial Control Agent | Lot Control, Serial No Control (traceability) |
| 6 | Shipping & Freight Agent | Shipper, Freight Category |
| 7 | Stock Document & Count Rules Agent | movement/adjustment document types, Inventory Count Rule (setup only) |
> Covers the 52 INVENTORY windows. Category/Attribute, Product Master and Costing are separate agents.

## 6. Manufacturing
| # | Agent | Setting areas |
|---|---|---|
| 0 | Manufacturing Solution Agent | plans Manufacturing setup |
| R | Readiness Check Agent | item master, warehouses, BOM inputs present |
| 1 | Manufacturing Setup Agent | Manufacturing Setup (global parameters) |
| 2 | Work Centre & Resource Agent | Production Resource, Resource Type, work centres, capacity/cost rates |
| 3 | Bill of Materials Agent | Bill of Material, Assembly/Manufacturing BOM, components |
| 4 | Routing & Operations Agent | routings, operations, standard times, Production Rule |
| 5 | Production Order Setup Agent | Production Order document types, backflush/issue-receipt rules (setup only) |
> Covers the 19 MANUFACTURING windows (Production Execution/Time Ticket are runtime, excluded).

## 7. Human Resources — **split into focused tiles** (HR has ~100 windows; one tile would overload)
> Recommend separate module tiles. Each tile = Solution + Readiness + the agents below.

**7a. HR Core**
| # | Agent | Setting areas |
|---|---|---|
| 1 | Org Structure Agent | Department, Position, Position Category, reporting lines |
| 2 | Job & Grade Agent | Job Family, Job Master, Grade Master, Grade Rate |
| 3 | Employee Master Agent | employee records (core identity & lifecycle states) |
| 4 | Employee Data Profiles Agent | personal/work info, dependents, bank, documents, skills & competency |
| 5 | Contract & Probation Agent *(optional)* | Contract Category/Master, probation rules |

**7b. Payroll**
| # | Agent | Setting areas |
|---|---|---|
| 1 | Payroll Periods Agent | Payroll Periods / payroll calendar |
| 2 | Pay Elements Agent | earnings, deductions, employer contributions |
| 3 | Statutory & Tax Agent | income tax, statutory deductions (links Finance withholding) |
| 4 | Payroll Cost Distribution Agent ⚠ | Payroll Cost Distribution Setup, Salary Bank Payment setup — *gated* |

**7c. Time & Attendance**
| # | Agent | Setting areas |
|---|---|---|
| 1 | Shift Definition Agent | Define Shift, Enrolled Shift |
| 2 | Attendance Rules Agent | Attendance Configuration, Attendance Rule |
| 3 | Overtime Setup Agent | overtime rules/policies |

**7d. Leave Management** — Leave Type Agent · Leave Accrual & Config Agent
**7e. Claims & Expense** — Claim Type Agent · Claim Eligibility Agent · Expense/Receipt Type Agent
**7f. Performance** — Performance Criteria Agent · Performance Measure Agent · Performance Goal/Plan Agent · Color Schema Agent
**7g. Recruitment** — Recruitment Template Agent · Questionnaire Agent
**7h. Employee Self-Service** — ESS Configuration Agent
> Covers the ~100 HR windows without overloading any agent. (Runtime — payroll runs, leave requests, claim submissions, attendance logs — are excluded.)

## 8. Projects
| # | Agent | Setting areas |
|---|---|---|
| 0 | Projects Solution Agent | plans Projects setup |
| R | Readiness Check Agent | accounting, customer master, resources present |
| 1 | Project Type & Template Agent | project types, Project Templates |
| 2 | Task & Phase Structure Agent | Task Master, Task Group, phases, milestones, Checklist Task |
| 3 | Project Costing Agent | cost categories, budgets, cost rates |
| 4 | Project Billing Agent | billing methods, rates, invoicing rules |
> Covers the 14 PROJECTS windows.

---

## Additional module tiles needed for FULL ERP coverage (found in metadata, not in the first 8)
| Module | Key setup agents |
|---|---|
| **Fixed Assets** | Asset Group Agent · Asset Register Agent · Depreciation Method Agent · Disposal/Transfer Rules Agent |
| **Service & Maintenance** | Equipment Master Agent · Maintenance Category Agent · Service Level Agent · Service Contract Agent · Work Order Class & Sort Agent · Maintenance Schedule Setup Agent |
| **Tax (if standalone)** | (otherwise lives inside Finance — Tax Setup + Withholding agents) |

## Cross-module implementation sequence (module order)
```
Basic Setup → Finance → Tax (in Finance) → Inventory → Procurement → Sales & CRM
   → Manufacturing → HR Core → Payroll / Time & Attendance / Leave / Claims
   → Projects → Fixed Assets → Service & Maintenance
```
Each module's Readiness Check Agent gates on the prior modules' outputs.

## Coverage check — how to keep it complete
Each module's agents are mapped to the live setup windows (BASIC 29, FINANCE 47, SALES/CRM 27, PROCURE 20,
INVENTORY 52, MFG 19, HR ~100, PROJECTS 14, SERVICE 28). When new setup windows appear in a metadata
refresh, add or extend the owning agent — never let one agent absorb an unrelated area.

## Mapping to existing build
- **Task agents = the Specialists** already prompt-built where they exist (Chart of Accounts, Accounting
  Schema, Posting Defaults). Reuse those prompts; build the rest on the same §10 consultant template.
- **Solution Agent** = lightweight planner (no tool delegation). **Readiness Check Agent** = Discovery scan.
- Agents marked ⚠ are high-impact → keep confirm + approval gating (Build Standards §D, §10.2).
