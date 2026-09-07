# Generates the Onfinity Smart API developer & integration guide (PDF). v2.6
from reportlab.lib.pagesizes import A4
from reportlab.lib.units import cm
from reportlab.lib import colors
from reportlab.lib.styles import getSampleStyleSheet, ParagraphStyle
from reportlab.lib.enums import TA_LEFT
from reportlab.platypus import (SimpleDocTemplate, BaseDocTemplate, PageTemplate, Frame,
                                Paragraph, Spacer, Preformatted,
                                Table, TableStyle, PageBreak, HRFlowable, KeepTogether)
from reportlab.platypus.tableofcontents import TableOfContents

OUT = r"C:\VA Standard\StandardSystem\SmartAPI\docs\Onfinity-SmartAPI-Developer-Guide.pdf"

BLUE   = colors.HexColor("#0083DA")
DARK   = colors.HexColor("#1f2d3d")
GREY   = colors.HexColor("#5b6b7b")
GRID   = colors.HexColor("#d8dee4")
CODEBG = colors.HexColor("#f4f6f8")
ZEBRA  = colors.HexColor("#f7f9fb")

styles = getSampleStyleSheet()
H1 = ParagraphStyle("H1", parent=styles["Heading1"], fontName="Helvetica-Bold",
                    fontSize=15, textColor=BLUE, spaceBefore=10, spaceAfter=6)
H2 = ParagraphStyle("H2", parent=styles["Heading2"], fontName="Helvetica-Bold",
                    fontSize=11.5, textColor=DARK, spaceBefore=11, spaceAfter=4)
H3 = ParagraphStyle("H3", parent=styles["Heading3"], fontName="Helvetica-Bold",
                    fontSize=9.6, textColor=BLUE, spaceBefore=9, spaceAfter=2)
GRP = ParagraphStyle("GRP", parent=styles["Heading1"], fontName="Helvetica-Bold",
                     fontSize=12.5, textColor=BLUE, spaceBefore=16, spaceAfter=5)
ENT = ParagraphStyle("ENT", parent=styles["Heading2"], fontName="Helvetica-Bold",
                     fontSize=11, textColor=DARK, spaceBefore=10, spaceAfter=3)
BODY = ParagraphStyle("BODY", parent=styles["BodyText"], fontName="Helvetica",
                      fontSize=9, leading=12.5, spaceAfter=4, alignment=TA_LEFT)
SMALL = ParagraphStyle("SMALL", parent=BODY, fontSize=8, leading=10.5, textColor=GREY, spaceAfter=2)
TAG = ParagraphStyle("TAG", parent=BODY, fontName="Courier-Bold", fontSize=8.5, textColor=DARK, spaceAfter=2)
CODE = ParagraphStyle("CODE", parent=styles["Code"], fontName="Courier", fontSize=7.4,
                      leading=9.2, backColor=CODEBG, borderColor=GRID, borderWidth=0.5,
                      borderPadding=5, spaceBefore=2, spaceAfter=6)
CELL = ParagraphStyle("CELL", fontName="Helvetica", fontSize=7.4, leading=9.2)
CELLH = ParagraphStyle("CELLH", fontName="Helvetica-Bold", fontSize=7.4, leading=9.2, textColor=colors.white)

def esc(s): return s.replace("&", "&amp;").replace("<", "&lt;").replace(">", "&gt;")
def code(s): return Preformatted(esc(s), CODE)
def mono(s): return '<font face="Courier">%s</font>' % esc(s)

story = []
def P(t, st=BODY): story.append(Paragraph(t, st))
def SP(h=6): story.append(Spacer(1, h))

def table(rows, widths, header=True):
    data = [[Paragraph(c, CELLH if (header and i == 0) else CELL) for c in row] for i, row in enumerate(rows)]
    t = Table(data, colWidths=widths, repeatRows=1 if header else 0)
    ts = [("GRID", (0, 0), (-1, -1), 0.4, GRID), ("VALIGN", (0, 0), (-1, -1), "TOP"),
          ("LEFTPADDING", (0, 0), (-1, -1), 4), ("RIGHTPADDING", (0, 0), (-1, -1), 4),
          ("TOPPADDING", (0, 0), (-1, -1), 2.5), ("BOTTOMPADDING", (0, 0), (-1, -1), 2.5)]
    if header:
        ts.append(("BACKGROUND", (0, 0), (-1, 0), BLUE))
        for r in range(2, len(rows), 2):
            ts.append(("BACKGROUND", (0, r), (-1, r), ZEBRA))
    t.setStyle(TableStyle(ts))
    story.append(t); story.append(Spacer(1, 7))

def func(title, verb_path, desc, req, resp, notes=None):
    """A function block: heading, method+path, description, sample request + response."""
    blk = []
    blk.append(Paragraph(title, H3))
    blk.append(Paragraph(esc(verb_path), TAG))
    if desc: blk.append(Paragraph(desc, BODY))
    if req is not None:
        blk.append(Paragraph("Request", SMALL)); blk.append(Preformatted(esc(req), CODE))
    blk.append(Paragraph("Response", SMALL)); blk.append(Preformatted(esc(resp), CODE))
    if notes: blk.append(Paragraph(notes, SMALL))
    story.append(KeepTogether(blk)); SP(2)

fw = [3.0*cm, 1.5*cm, 1.1*cm, 11.4*cm]  # entity field table widths

# ---------------------------------------------------------------- cover
story.append(Spacer(1, 5*cm))
P("Onfinity Smart API", ParagraphStyle("T", parent=H1, fontSize=30, spaceAfter=4))
P("Developer &amp; Integration Guide", ParagraphStyle("ST", parent=H2, fontSize=15, textColor=GREY))
SP(18)
P("Identifier-based API for Onfinity (ViennaAdvantage). Create, update, read and act on master "
  "data, CRM records and sales documents using business identifiers instead of internal primary keys.",
  ParagraphStyle("LEAD", parent=BODY, fontSize=11, leading=16))
SP(34)
table([["Version", "2.6"], ["Date", "13 July 2026"],
       ["Audience", "Integration developers and AI integration agents"],
       ["Entities", "58 - basic setup, financial, master data, CRM, sales, HR/payroll (Section 9)"],
       ["Capabilities", "CRUD + retrieval (filter/sort/page/select) + nested writes + actions"]],
      [3.4*cm, 13.6*cm], header=False)
SP(16)
P("Every function in Section 6 includes a sample request and response. Live source of truth: "
  + mono("GET /v1/_entities") + " and " + mono("/swagger") + ".", SMALL)
story.append(PageBreak())

# ---------------------------------------------------------------- table of contents
P("Contents", ParagraphStyle("TOCH", parent=H1, textColor=BLUE))
toc = TableOfContents()
toc.levelStyles = [
    ParagraphStyle("TOC0", fontName="Helvetica-Bold", fontSize=10, leading=16, textColor=DARK, spaceBefore=4),
    ParagraphStyle("TOC1", fontName="Helvetica-Bold", fontSize=9, leading=13, leftIndent=14, textColor=BLUE),
    ParagraphStyle("TOC2", fontName="Helvetica", fontSize=8.5, leading=12, leftIndent=30, textColor=GREY),
]
story.append(toc)
story.append(PageBreak())

# ---------------------------------------------------------------- 1 overview
P("1. Overview", H1)
P("The Onfinity Smart API is a thin, stateless REST layer in front of Onfinity's core API (VAAPI). "
  "Callers work with <b>business identifiers</b> (search keys, names, ISO codes); the API resolves them "
  "to internal foreign keys and performs the insert, update, read, or process call through Onfinity.")
table([
    ["Capability", "Detail"],
    ["Identifier-based", "Send business keys (e.g. " + mono('group: "Employees"') + "); the API resolves the IDs."],
    ["CRUD", "Create / upsert / partial-update / read, by business key."],
    ["Retrieval", "List / search by identifier filters, with paging, sorting and field selection."],
    ["Nested writes", "Create a parent and its children (and owned sub-records) in one call."],
    ["Window processes", "Run record actions (e.g. Lead -> Prospect, complete a quotation)."],
    ["Dry run", "Validate + resolve a write without changing anything."],
    ["Stateless", "No database; manifests are files; all data lives in Onfinity."],
    ["Authorization", "Every call runs under the caller's Onfinity role; the API adds no privilege."],
], [3.4*cm, 13.6*cm])

# ---------------------------------------------------------------- 2 base url
P("2. Base URL, hosting and docs", H1)
P("The Smart API is deployed alongside an Onfinity installation; obtain the base URL from your "
  "administrator. All paths are relative to it. An interactive Swagger UI is at " + mono("/swagger") +
  " and the OpenAPI document at " + mono("/swagger/v1/swagger.json") + ".")
code("On-prem (IIS):    http://{onfinity-host}:8090\n"
     "Container (Docker): http://{linux-host}:8090")
P("<b>Use HTTPS in production</b> - credentials travel in headers.", SMALL)

# ---------------------------------------------------------------- 3 auth
P("3. Authentication", H1)
P("Every request (except " + mono("/health") + " and " + mono("/swagger") + ") sends two headers, "
  "the Onfinity VAAPI key pair. They are forwarded unchanged, so each call runs under that key's role "
  "- the API cannot exceed what the role permits.")
table([["Header", "Description"],
       ["accessKey", "VAAPI access key (issued by Onfinity)."],
       ["secretKey", "VAAPI secret key (issued by Onfinity)."]], [3.6*cm, 13.4*cm])
code('curl -H "accessKey: $AK" -H "secretKey: $SK" http://{host}:8090/v1/_entities')
P("Missing/empty keys -> <b>401 NO_CREDENTIALS</b>. Resolving an identifier needs read access to the "
  "referenced table; otherwise the reference returns MISSING_REF.", SMALL)

# ---------------------------------------------------------------- 4 concepts
P("4. Core concepts", H1)
table([
    ["Concept", "Meaning"],
    ["Business key", "The " + mono("key") + " field uniquely identifies a record (maps to Value or DocumentNo). Reads/updates/actions address records by it."],
    ["Reference (ref)", "A field whose value is a related record's identifier; resolved to a foreign key. Must match exactly one active record."],
    ["List", "A human label mapped to an internal code (e.g. productType \"Item\" -> I)."],
    ["Derived field", "Auto-filled from a related row, no caller input (e.g. a quotation's org/location from the partner)."],
    ["Owned sub-record", "A nested object created first so its id fills a foreign key (e.g. an address)."],
    ["Children", "One-to-many collections sent as arrays; the parent link is set automatically."],
    ["Key by reference", "Some one-to-one records are keyed by a related record's identifier (e.g. EmployeeStatutoryInfo is keyed by " + mono("businessPartner") + ") - send that identifier as the key."],
    ["Booleans / dates", "Booleans accept true/false (stored Y/N). Dates are ISO strings (YYYY-MM-DD)."],
    ["Dry run", "Append " + mono("?dryRun=true") + " to a write to validate + resolve without changing data."],
], [3.4*cm, 13.6*cm])

# ---------------------------------------------------------------- 5 responses
P("5. Response shapes", H1)
P("Writes return the mode, business key, internal id and (if sent) the child records. Dry runs add "
  "the resolved column values and a children plan. Errors return an " + mono("error") + " object.")
code('// write                       // dry run                        // error\n'
     '{ "mode": "created",          { "mode": "would-create",        { "error": {\n'
     '  "key": "ACME-001",            "key": "ACME-001",                "code": "MISSING_REF",\n'
     '  "recordId": 1041220,          "recordId": 0,                    "message": "..." } }\n'
     '  "children": { ... } }         "dryRun": true,\n'
     '                                "resolved": { ... },\n'
     '                                "children": { ... } }')

# ================================================================ 6 FUNCTIONS
story.append(PageBreak())
P("6. Functions", H1)
P("Every function below shows a sample request and response. " + mono("{entity}") +
  " is a registered entity (Section 8); " + mono("{key}") + " is its business key.")

P("6.1 Health check", H2)
func("Liveness", "GET /health",
     "No authentication. Confirms the service is up.",
     None, '{ "status": "ok" }')

P("6.2 List entities", H2)
func("List registered entities", "GET /v1/_entities",
     "Returns the entity names the API serves.",
     None,
     '[ "BusinessPartner", "Campaign", "Lead", "Product",\n'
     '  "Opportunity", "Request", "SalesQuotation" ]')

P("6.3 Create (insert-only)", H2)
func("Create a record", "POST /v1/{entity}",
     "Inserts a new record. Fails with 409 if the business key already exists. "
     "Identifiers are resolved to ids; unsupplied fields use Onfinity model defaults.",
     '{\n  "key": "SKU-100", "name": "Widget",\n  "productType": "Item",\n'
     '  "category": "1000001", "uom": "Each"\n}',
     '{\n  "mode": "created",\n  "key": "SKU-100",\n  "recordId": 1041220,\n'
     '  "dryRun": false,\n  "children": null\n}',
     "POST is insert-only. To create-or-update, use PUT (6.4).")

P("6.4 Upsert", H2)
func("Create or update by key", "PUT /v1/{entity}",
     "Inserts if the key is new, otherwise updates. Idempotent - safe to retry.",
     '{\n  "key": "CUST-0042", "name": "Acme GmbH",\n  "isCustomer": true,\n'
     '  "group": "Employees",\n  "priceList": "Gold Partner Price List",\n  "paymentTerm": "Immediate"\n}',
     '{\n  "mode": "created",          // or "updated"\n  "key": "CUST-0042",\n'
     '  "recordId": 1041220\n}')

P("6.5 Partial update", H2)
func("Update selected fields", "PATCH /v1/{entity}/{key}",
     "Updates only the supplied fields of an existing record. Common use: deactivate a record.",
     '{ "isActive": false }',
     '{\n  "mode": "updated",\n  "key": "CUST-0042",\n  "recordId": 1041220\n}')

P("6.6 List / search", H2)
func("List records (filter, page, sort, project)", "GET /v1/{entity}",
     "Query a collection. Filters use the same field names as writes (identifiers in, identifiers out). "
     "Supports paging, sorting and field selection. Returns identifiers by default; add " + mono("?ids=true") +
     " to also include raw foreign-key ids.",
     '// GET /v1/BusinessPartner?isCustomer=true&group=Customers\n'
     '//        &sort=name&fields=key,name,group&page=1&pageSize=50',
     '{\n  "page": 1, "pageSize": 50, "total": 2569,\n  "items": [\n'
     '    { "key": "CUST-0042", "recordId": 1041220,\n'
     '      "name": "Acme GmbH", "group": "Customers" }\n  ]\n}')
table([
    ["Query parameter", "Effect"],
    [mono("field=value"), "filter by any field using its identifier (e.g. group=Customers, isCustomer=true). Text supports a trailing * for prefix match."],
    [mono("page") + " / " + mono("pageSize"), "paging (pageSize default 50, max 200). Response includes total."],
    [mono("sort=field") + " / " + mono("sort=-field"), "sort ascending / descending; comma-separate for multiple keys."],
    [mono("fields=a,b,c"), "return only these fields (plus key and recordId)."],
    [mono("ids=true"), "also include raw foreign-key ids next to the names (as " + mono("<field>Id") + ")."],
], [4.0*cm, 13.0*cm])

P("6.7 Read one", H2)
func("Read a record", "GET /v1/{entity}/{key}",
     "Returns the record with foreign keys mapped to identifiers and list codes to labels. Options: "
     + mono("?include=children") + " embeds child collections; " + mono("?ids=true") + " adds raw fk ids; "
     + mono("?fields=a,b") + " projects selected fields.",
     None,
     '// GET /v1/Product/VISP0001?include=children\n{\n  "key": "VISP0001",\n  "name": "5 Seater Sofa",\n'
     '  "productType": "Item",        // code I -> label\n'
     '  "category": "1000001",        // id  -> M_Product_Category.Value\n'
     '  "uom": "Each", "taxCategory": "Standard",\n'
     '  "children": {\n    "vendors": [ { "recordId": 5012, "vendor": "VEND-01", "poPrice": "420.0" } ],\n'
     '    "substitutes": [], "relatedProducts": []\n  }\n}')

P("6.8 Dry run", H2)
func("Validate without writing", "POST|PUT|PATCH /v1/{entity}?dryRun=true",
     "Performs all validation and identifier resolution and returns exactly what would be written - "
     "no change is made. Use it to verify a payload (catches MISSING_REF / MISSING_FIELD safely).",
     '{ "key": "VAI159", "name": "Sunny", "group": "Employees",\n'
     '  "priceList": "Gold Partner Price List", "country": "DE" }',
     '{\n  "mode": "would-update",\n  "key": "VAI159",\n  "recordId": 1040878,\n  "dryRun": true,\n'
     '  "resolved": {\n    "Name": "Sunny",\n    "C_BP_Group_ID": 1000035,   // "Employees"\n'
     '    "M_PriceList_ID": 1000183,  // "Gold Partner Price List"\n'
     '    "C_Country_ID": 101         // "DE"\n  }\n}')

P("6.9 Nested write (children + owned records)", H2)
func("Create a parent with children in one call", "POST /v1/{entity}",
     "Send one-to-many relations as arrays; the parent link is auto-filled. An owned record (e.g. an "
     "address) is a nested object created first so its id fills a foreign key.",
     '{\n  "key": "ACME-001", "name": "Acme GmbH",\n  "isCustomer": true, "group": "Employees",\n'
     '  "contacts": [\n    { "name": "Jane Doe", "email": "jane@acme.example" }\n  ],\n'
     '  "locations": [\n    { "name": "HQ",\n'
     '      "address": { "address1": "Hauptstr 1", "city": "Berlin",\n'
     '                   "postal": "10115", "country": "DE" } }\n  ]\n}',
     '{\n  "mode": "created",\n  "key": "ACME-001",\n  "recordId": 1041240,\n'
     '  "children": {\n    "contacts":  [ { "table": "AD_User", "id": 1005512 } ],\n'
     '    "locations": [ { "table": "C_BPartner_Location", "id": 2003318 } ]\n  }\n}')

P("6.10 List actions", H2)
func("List window processes on an entity", "GET /v1/{entity}/_actions",
     "Returns the record processes (actions) the entity supports.",
     None,
     '// GET /v1/Lead/_actions\n[\n'
     '  { "action": "generateProspect",    "label": "Generate Prospect",    "params": [] },\n'
     '  { "action": "generateOpportunity", "label": "Generate Opportunity", "params": [] },\n'
     '  { "action": "createRequest",       "label": "Create Request",       "params": [] },\n'
     '  { "action": "addToTargetList",     "label": "Add To Target List",   "params": [] }\n]')

P("6.11 Run an action", H2)
func("Run a window process on a record", "POST /v1/{entity}/{key}/actions/{action}",
     "Resolves the record by key, then runs the process (optionally with parameters in the body). "
     "Returns the process message.",
     '// POST /v1/Lead/LEAD-001/actions/generateProspect\n'
     '{ }                                  // params if the process needs them',
     '{\n  "action": "generateProspect",\n  "key": "LEAD-001",\n  "recordId": 1000451,\n'
     '  "message": "Process executed successfully"\n}',
     "Examples: complete a quotation -> POST /v1/SalesQuotation/{key}/actions/complete ; "
     "add a partner to a target list -> .../BusinessPartner/{key}/actions/addToTargetList.")
P("Menu processes (not bound to a record, e.g. Tenant " + mono("initialSetup") + ") use the keyless form "
  + mono("POST /v1/{entity}/actions/{action}") + " with parameters in the body.", SMALL)

# ---------------------------------------------------------------- 7 field behaviours
story.append(PageBreak())
P("7. Field behaviours", H1)
table([
    ["Behaviour", "How it works (caller's view)"],
    ["Reference", "Send the related record's identifier (e.g. " + mono('group: "Employees"') + "). Resolved to the FK. 0 matches -> MISSING_REF; many -> AMBIGUOUS_REF."],
    ["List", "Send the label exactly as listed for the field (e.g. " + mono('productType: "Item"') + "). Wrong value -> BAD_LIST_VALUE."],
    ["Derived", "Filled automatically from a related record - you do not send it (e.g. a quotation's organisation and bill-to location come from the partner)."],
    ["Manual vs list price", "On a quotation line, send " + mono("price") + " to USE that price; omit it to let the Onfinity price list set the price."],
    ["Defaults", "Required fields must be sent on insert. Omitted optional fields take Onfinity's model defaults."],
], [3.6*cm, 13.4*cm])
P("Manual-price example (line price is used as sent):", SMALL)
code('"lines": [ { "product": "VISP0001", "warehouse": "Vienna Warehouse",\n'
     '             "qty": 2, "price": 499.00 } ]   // PriceActual becomes 499.00')

# ---------------------------------------------------------------- 8 errors
P("8. Errors", H1)
code('{ "error": { "code": "MISSING_REF",\n'
     '             "message": "\'group\' = \'NoSuchGroup\' not found in C_BP_Group.Value." } }')
table([
    ["Code", "HTTP", "Meaning"],
    ["NO_CREDENTIALS", "401", "accessKey / secretKey header missing."],
    ["UNKNOWN_ENTITY / UNKNOWN_ACTION", "404", "No such entity / action."],
    ["NOT_FOUND", "404", "Record (by key) does not exist."],
    ["MISSING_KEY / MISSING_FIELD / MISSING_PARAM", "400/422", "A required key / field / action parameter is absent."],
    ["ALREADY_EXISTS", "409", "POST insert, but the key already exists."],
    ["AMBIGUOUS_KEY / AMBIGUOUS_REF", "409/422", "Key or reference matched more than one record."],
    ["MISSING_REF", "422", "A reference value matched no active record."],
    ["BAD_LIST_VALUE / BAD_NUMBER / TOO_LONG", "422", "A value is not a valid label / number / within length."],
    ["VAAPI_ERROR", "502", "The underlying Onfinity call or process failed (message included)."],
], [5.0*cm, 1.5*cm, 10.5*cm])

# ================================================================ 9 ENTITIES
story.append(PageBreak())
P("9. Entity reference", H1)
P("Each entity is addressed by name (e.g. " + mono("/v1/Lead") + "). Tables show the API field, type, "
  "whether required on insert, and how it maps/resolves in Onfinity. Refs/lists carry the resolution. "
  "See " + mono("/swagger") + " for the exhaustive field list.")

def entity(name, table_name, key_desc, rows, children=None, actions=None, note=None):
    P(name, ENT)
    P("Onfinity table " + mono(table_name) + ". Key: " + key_desc + ".", BODY)
    if note: P(note, SMALL)
    table([["Field", "Type", "Req", "Maps to / resolution"]] + rows, fw)
    if children:
        P("Children", H3)
        for cn, cdesc in children:
            P(mono(cn) + " - " + cdesc, BODY)
    if actions:
        P("Actions  (POST /v1/" + name + "/{key}/actions/{action})", H3)
        table([["action", "Process / effect"]] + actions, [4.0*cm, 13.0*cm])

P("Basic setup", GRP)

entity("Tenant", "AD_Client", mono("key") + " = tenant name (no Value column)",
    [[mono("key"), "string", "Yes", "Name"],
     [mono("description"), "string", "-", "Description"],
     [mono("isActive"), "boolean", "-", "IsActive (default true)"]],
    actions=[[mono("initialSetup"), "MENU action - fully provisions a tenant (client + org + users + roles + accounting + calendar) via MSetup.CreateClient. Params: ClientName, OrgName, AdminUserName, NormalUserName. Call: POST /v1/Tenant/actions/initialSetup"]],
    note="A raw insert creates only the AD_Client row; full provisioning is the initialSetup process.")

entity("Organization", "AD_Org", mono("key") + " = search key (Value)",
    [[mono("key"), "string", "Yes", "Value"],
     [mono("name"), "string", "Yes", "Name"],
     [mono("isSummary"), "boolean", "-", "IsSummary (default false)"],
     [mono("salesRegion"), "ref", "-", "C_SalesRegion by Value"]],
    note="AD_Client_ID comes from the calling session.")

entity("Role", "AD_Role", mono("key") + " = role name (no Value)",
    [[mono("key"), "string", "Yes", "Name"],
     [mono("userLevel"), "string", "-", "UserLevel mask, e.g. ' CO' (model defaults if omitted)"],
     [mono("isAdministrator / isAccessAllOrgs"), "boolean", "-", "default false"],
     [mono("isCanExport / isCanReport / isCanApproveOwnDoc"), "boolean", "-", "default true"],
     [mono("confirmQueryRecords"), "number", "-", "default 0"]],
    note="Raw insert = role row only; window/org access grants are separate (role window or Initial Client Setup).")

entity("User", "AD_User", mono("key") + " = search key (Value)",
    [[mono("key"), "string", "Yes", "Value"],
     [mono("name"), "string", "Yes", "Name"],
     [mono("email"), "string", "Yes", "EMail"],
     [mono("phone / description"), "string", "-", "Phone / Description"],
     [mono("role"), "ref", "-", "AD_Role by Name"],
     [mono("businessPartner"), "ref", "-", "C_BPartner by Value"],
     [mono("greeting"), "ref", "-", "C_Greeting by Name"]],
    note="Login enabling / password are handled separately.")

entity("Calendar", "C_Calendar", mono("key") + " = calendar name (no Value)",
    [[mono("key"), "string", "Yes", "Name"],
     [mono("description"), "string", "-", "Description"]])

entity("FiscalYear", "C_Year", mono("key") + " = the year, e.g. '2026' (FiscalYear)",
    [[mono("key"), "string", "Yes", "FiscalYear (unique per calendar)"],
     [mono("calendar"), "ref", "Yes", "C_Calendar by Name"],
     [mono("description"), "string", "-", "Description"]],
    children=[("periods[]", "C_Period: name(req), periodNo(req), startDate(req), endDate, periodType(Standard | Adjustment)")],
    actions=[[mono("createPeriods"), "auto-generate the year's periods (optional 'month' param)"]],
    note="The key is scoped by calendar - pass 'calendar' in the body of reads/actions to select the right year.")

entity("Country", "C_Country", mono("key") + " = ISO country code",
    [[mono("key"), "string", "Yes", "CountryCode (ISO alpha-2, e.g. DE)"],
     [mono("name"), "string", "Yes", "Name"],
     [mono("regionName"), "string", "-", "RegionName (label for the 'region' field)"],
     [mono("salesRegion"), "ref", "-", "C_SalesRegion by Value"]],
    children=[("regions[]", "C_Region: name(req), isDefault")])

entity("City", "C_City", mono("key") + " = city name (scoped by country)",
    [[mono("key"), "string", "Yes", "Name"],
     [mono("country"), "ref", "-", "C_Country by CountryCode"],
     [mono("postal"), "string", "-", "Postal"]],
    note="City names are not unique - the key is scoped by country; pass 'country' to disambiguate.")

entity("SalesRegion", "C_SalesRegion", mono("key") + " = search key (Value)",
    [[mono("key"), "string", "Yes", "Value (what 'salesRegion' refs resolve)"],
     [mono("name"), "string", "Yes", "Name"],
     [mono("isSummary / isDefault"), "boolean", "-", "default false"]])

entity("Greeting", "C_Greeting", mono("key") + " = greeting name (no Value)",
    [[mono("key"), "string", "Yes", "Name (what 'greeting' refs resolve)"],
     [mono("greeting"), "string", "-", "Greeting text printed on correspondence"],
     [mono("isFirstNameOnly / isDefault"), "boolean", "-", "default false"]])

entity("Currency", "C_Currency", mono("key") + " = ISO currency code",
    [[mono("key"), "string", "Yes", "ISO_Code (3 letters, e.g. EUR)"],
     [mono("description"), "string", "Yes", "Description"],
     [mono("symbol"), "string", "-", "CurSymbol"],
     [mono("stdPrecision"), "number", "-", "StdPrecision (default 2)"],
     [mono("costingPrecision"), "number", "-", "CostingPrecision (default 4)"],
     [mono("isEuro / isEMUMember"), "boolean", "-", "IsEuro / IsEMUMember (default false)"]])

entity("ConversionType", "C_ConversionType", mono("key") + " = search key (Value)",
    [[mono("key"), "string", "Yes", "Value (rate type, e.g. Spot)"],
     [mono("name"), "string", "Yes", "Name"],
     [mono("isDefault"), "boolean", "-", "default false"]])

entity("CurrencyRate", "C_Conversion_Rate", mono("validFrom") + " = ISO date (scoped key)",
    [[mono("validFrom"), "string", "Yes", "ValidFrom (the key)"],
     [mono("fromCurrency"), "ref", "Yes", "C_Currency by ISO_Code (source)"],
     [mono("toCurrency"), "ref", "Yes", "C_Currency by ISO_Code (target)"],
     [mono("conversionType"), "ref", "Yes", "C_ConversionType by Name"],
     [mono("multiplyRate"), "number", "Yes", "source * rate = target (divideRate auto-derived)"],
     [mono("validTo"), "string", "-", "ISO date"]],
    note="No single key - identified by (fromCurrency, toCurrency, conversionType, validFrom). Key = validFrom, scoped by the pair + type.")

entity("UOM", "C_UOM", mono("key") + " = name (no Value)",
    [[mono("key"), "string", "Yes", "Name (what 'uom' refs resolve)"],
     [mono("uomCode"), "string", "Yes", "X12DE355 (EDI UOM code, e.g. EA)"],
     [mono("symbol"), "string", "-", "UOMSymbol"],
     [mono("stdPrecision / costingPrecision"), "number", "-", "default 0"],
     [mono("isDefault"), "boolean", "-", "default false"]])

entity("DocType", "C_DocType", mono("key") + " = document type name",
    [[mono("key"), "string", "Yes", "Name (what 'docType' refs resolve)"],
     [mono("value"), "string", "-", "Value (optional search key)"],
     [mono("docBaseType"), "list", "Yes", "Sales Order | Purchase Order | AR Invoice | AP Invoice | AR/AP Credit Memo | AR Receipt | AP Payment | Material Shipment | Material Receipt | Material Movement | Physical Inventory | GL Journal | GL Document"],
     [mono("glCategory"), "ref", "Yes", "GL_Category by Name"],
     [mono("isSOTrx"), "boolean", "-", "IsSOTrx"],
     [mono("documentCopies"), "number", "-", "DocumentCopies (default 1)"],
     [mono("hasCharges / isDefault"), "boolean", "-", "default false"]])

entity("Sequence", "AD_Sequence", mono("key") + " = sequence name (no Value)",
    [[mono("key"), "string", "Yes", "Name"],
     [mono("description"), "string", "-", "Description"],
     [mono("prefix / suffix"), "string", "-", "Prefix / Suffix"],
     [mono("currentNext"), "number", "-", "CurrentNext (default 1000000)"],
     [mono("incrementNo"), "number", "-", "IncrementNo (default 1)"],
     [mono("isAutoSequence"), "boolean", "-", "default true"]],
    note="Document / number sequence. System counters are model-defaulted.")

entity("Tree", "AD_Tree", mono("key") + " = tree name",
    [[mono("key"), "string", "Yes", "Name (what Element 'tree' resolves)"],
     [mono("treeType"), "list", "Yes", "Element Value | Organization | BPartner | Product | Campaign | Activity | Sales Region | Project | Menu | ..."],
     [mono("table"), "ref", "Yes", "AD_Table by TableName (the tree's table, e.g. C_ElementValue)"],
     [mono("isAllNodes / isDefault"), "boolean", "-", "default false"]],
    note="A hierarchy tree. Records of tree-supported tables auto-attach as nodes on insert.")

P("Financial &amp; accounting", GRP)

entity("AccountingSchema", "C_AcctSchema", mono("key") + " = schema name",
    [[mono("key"), "string", "Yes", "Name"],
     [mono("currency"), "ref", "Yes", "C_Currency by ISO_Code"],
     [mono("costingMethod"), "list", "-", "Standard Costing | Average PO | Average Invoice | Fifo | Lifo | Weighted Average Cost | Last Invoice | Last PO Price"],
     [mono("costingLevel"), "list", "-", "Client | Organization | Warehouse | Batch/Lot"],
     [mono("gaap"), "list", "-", "International GAAP | German HGB | French Accounting Standard"],
     [mono("autoPeriodControl / isAccrual"), "boolean", "-", "default true"]],
    note="A raw insert creates the header only - NOT a usable schema. The real one (account elements, default accounts, GL categories) is built by Initial Client Setup.")

entity("GLCategory", "GL_Category", mono("key") + " = name (no Value)",
    [[mono("key"), "string", "Yes", "Name (what DocType 'glCategory' resolves)"],
     [mono("categoryType"), "list", "Yes", "Document | Manual | Imported | Greeting"],
     [mono("isDefault"), "boolean", "-", "default false"]])

entity("TaxCategory", "C_TaxCategory", mono("key") + " = name (no Value)",
    [[mono("key"), "string", "Yes", "Name"],
     [mono("isDefault"), "boolean", "-", "IsDefault (default false)"]])

entity("Tax", "C_Tax", mono("key") + " = tax name",
    [[mono("key"), "string", "Yes", "Name"],
     [mono("rate"), "number", "Yes", "Rate (percentage, e.g. 19)"],
     [mono("taxCategory"), "ref", "-", "C_TaxCategory by Name"],
     [mono("country"), "ref", "-", "C_Country by CountryCode"],
     [mono("isSalesTax / isDocumentLevel / isTaxExempt / isDefault / isSummary"), "boolean", "-", "default false"]])

entity("Charge", "C_Charge", mono("key") + " = charge name",
    [[mono("key"), "string", "Yes", "Name (e.g. Handling, Shipping)"],
     [mono("chargeAmt"), "number", "-", "ChargeAmt (default 0)"],
     [mono("taxCategory"), "ref", "Yes", "C_TaxCategory by Name"],
     [mono("activity"), "ref", "-", "C_Activity by Value"],
     [mono("isSameTax / isSameCurrency / isTaxIncluded"), "boolean", "-", "isSameTax / isSameCurrency default true"]])

entity("Activity", "C_Activity", mono("key") + " = search key (Value)",
    [[mono("key"), "string", "Yes", "Value (billing code / activity-based costing)"],
     [mono("name"), "string", "Yes", "Name"],
     [mono("description"), "string", "-", "Description"],
     [mono("isSummary"), "boolean", "-", "default false"]])

entity("Bank", "C_Bank", mono("key") + " = bank name (no Value)",
    [[mono("key"), "string", "Yes", "Name"],
     [mono("routingNo"), "string", "Yes", "RoutingNo (ABA)"],
     [mono("isOwnBank"), "boolean", "-", "default true"]],
    children=[("bankAccounts[]", "C_BankAccount: accountNo(req), accountType(Checking | Savings), currency(ref), creditLimit, currentBalance, isDefault")])

entity("PaymentTerm", "C_PaymentTerm", mono("key") + " = search key (Value)",
    [[mono("key"), "string", "Yes", "Value (what 'paymentTerm' refs resolve)"],
     [mono("name"), "string", "Yes", "Name"],
     [mono("netDays"), "number", "-", "NetDays (days until due)"],
     [mono("discount / discountDays / graceDays"), "number", "-", "Discount / DiscountDays / GraceDays (default 0)"],
     [mono("afterDelivery / isDueFixed / isDefault"), "boolean", "-", "default false"]])

entity("Element", "C_Element", mono("key") + " = element name",
    [[mono("key"), "string", "Yes", "Name"],
     [mono("elementType"), "list", "Yes", "Account | User defined"],
     [mono("tree"), "ref", "Yes", "AD_Tree by Name (the account tree)"],
     [mono("isNaturalAccount"), "boolean", "-", "default true"],
     [mono("isBalancing"), "boolean", "-", "default false"],
     [mono("vformat"), "string", "-", "VFormat (value mask)"]],
    children=[("accounts[]", "C_ElementValue: value(req, account code), name(req), accountType(Asset|Expense|Liability|Owner's Equity|Revenue|Memo), accountSign(Natural|Debit|Credit), isSummary, accountGroup(ref C_AccountGroup by Name), parent(optional - a sibling account's code, to nest this account under it in the tree)")],
    note="Chart of accounts. Accounts auto-attach to the element's tree; send 'parent' on an account to nest it. The full chart is normally built by Initial Client Setup.")

entity("AccountGroup", "C_AccountGroup", mono("key") + " = name (no Value)",
    [[mono("key"), "string", "Yes", "Name (what an account's 'accountGroup' resolves)"],
     [mono("value"), "string", "-", "Value (optional search key)"]])

P("Master data", GRP)

entity("BusinessPartner", "C_BPartner", mono("key") + " = search key (Value)",
    [[mono("key"), "string", "Yes", "Value (upsert identity)"],
     [mono("name"), "string", "Yes", "Name"],
     [mono("isCustomer / isVendor / isProspect / isEmployee / isSalesRep"), "boolean", "-", "role flags (default false)"],
     [mono("taxId / duns / referenceNo / url"), "string", "-", "TaxID / DUNS / ReferenceNo / URL"],
     [mono("email / phone / mobile / fax"), "string", "-", "EMail / Phone / Mobile / Fax"],
     [mono("creditLimit / flatDiscount"), "number", "-", "SO_CreditLimit / FlatDiscount"],
     [mono("group"), "ref", "Yes", "C_BP_Group by Value"],
     [mono("priceList"), "ref", "-", "M_PriceList by Name"],
     [mono("paymentTerm"), "ref", "-", "C_PaymentTerm by Value"],
     [mono("country"), "ref", "-", "C_Country by CountryCode (ISO alpha-2)"],
     [mono("greeting / salesRegion"), "ref", "-", "C_Greeting by Name / C_SalesRegion by Value"],
     [mono("deliveryRule"), "list", "-", "Availability | Force | Complete Line | Manual | Complete Order"],
     [mono("invoiceRule"), "list", "-", "After Partial Delivery | Immediate | After Full Order Delivery | Customer Schedule after Delivery"]],
    children=[("contacts[]", "AD_User: name(req), email(req), phone, key"),
              ("locations[]", "C_BPartner_Location: name(req), isBillTo/isShipTo/isPayFrom/isRemitTo, phone, address(owned C_Location: address1/2, city, postal, regionName, country(req))")],
    actions=[[mono("generateOpportunity"), "create an opportunity from this (prospect) partner"],
             [mono("generateCustomer"), "generate a customer account"],
             [mono("addToTargetList"), "add the partner to a marketing target list"]])

entity("BPGroup", "C_BP_Group", mono("key") + " = search key (Value)",
    [[mono("key"), "string", "Yes", "Value (what BusinessPartner 'group' resolves)"],
     [mono("name"), "string", "Yes", "Name"],
     [mono("priceList"), "ref", "-", "M_PriceList by Name"],
     [mono("isDefault / isConfidentialInfo"), "boolean", "-", "default false"]])

entity("Product", "M_Product", mono("key") + " = search key (Value)",
    [[mono("key"), "string", "Yes", "Value"],
     [mono("name"), "string", "Yes", "Name"],
     [mono("description / help / sku / upc / make / model"), "string", "-", "Description / Help / SKU / UPC / Make / Model"],
     [mono("productType"), "list", "Yes", "Item | Service | Expense | Resource"],
     [mono("category"), "ref", "Yes", "M_Product_Category by Value"],
     [mono("uom"), "ref", "Yes", "C_UOM by Name (e.g. 'Each')"],
     [mono("taxCategory / brand"), "ref", "-", "C_TaxCategory by Name / M_Brand by Name"],
     [mono("isSold / isPurchased / isStocked"), "boolean", "-", "default true"],
     [mono("qtyTolerance"), "number", "-", "QtyTolerance"]],
    children=[("vendors[]", "M_Product_PO: vendor(req, C_BPartner by Value), currency(C_Currency ISO), uom, isCurrentVendor, listPrice, poPrice, orderMin, orderPack, priceEffective, manufacturer"),
              ("substitutes[]", "M_Substitute: name(req), substitute(req, product by Value), description"),
              ("relatedProducts[]", "M_RelatedProduct: name(req), relatedProduct(req, product by Value), relatedType(Alternative|Mandatory|Web Promotion|Supplemental), quantity, uom")],
    note="Also exposes optional inventory/manufacturing flags (isBOM, isManufactured, isDropShip, ...), default false.")

entity("ProductCategory", "M_Product_Category", mono("key") + " = search key (Value)",
    [[mono("key"), "string", "Yes", "Value (what 'category' refs resolve)"],
     [mono("name"), "string", "Yes", "Name"],
     [mono("mmPolicy"), "list", "-", "FiFo | LiFo"],
     [mono("taxCategory"), "ref", "-", "C_TaxCategory by Name"],
     [mono("isDefault / isPurchasedToOrder"), "boolean", "-", "default false"]])

entity("PriceList", "M_PriceList", mono("key") + " = name (no Value)",
    [[mono("key"), "string", "Yes", "Name (what 'priceList' refs resolve)"],
     [mono("currency"), "ref", "Yes", "C_Currency by ISO_Code"],
     [mono("isSOPriceList"), "boolean", "-", "IsSOPriceList (sales=true / purchase=false, default true)"],
     [mono("isTaxIncluded / enforcePriceLimit / isDefault"), "boolean", "-", "default false"],
     [mono("pricePrecision"), "number", "-", "PricePrecision (default 2)"]],
    note="Prices live in price-list versions (M_PriceList_Version) + M_ProductPrice, set up separately.")

entity("DiscountSchema", "M_DiscountSchema", mono("key") + " = name (no Value)",
    [[mono("key"), "string", "Yes", "Name"],
     [mono("discountType"), "list", "Yes", "Pricelist | Breaks | Flat Percent"],
     [mono("validFrom"), "string", "Yes", "ValidFrom (ISO date)"],
     [mono("isQuantityBased / isBPartnerFlatDiscount"), "boolean", "-", "default false"]],
    note="Discount breaks/lines are set up separately.")

entity("Warehouse", "M_Warehouse", mono("key") + " = search key (Value)",
    [[mono("key"), "string", "Yes", "Value (what 'warehouse' refs resolve)"],
     [mono("name"), "string", "Yes", "Name"],
     [mono("separator"), "string", "-", "Separator (default '-')"],
     [mono("isDisallowNegativeInv / isWMSEnabled"), "boolean", "-", "default false"],
     [mono("address"), "owned", "Yes", "owned C_Location: address1, city, postal, country(req)"]],
    children=[("locators[]", "M_Locator: value(req), x/y/z (aisle/bin/level), isDefault, priorityNo(default 50), pickingSeqNo, putawaySeqNo, isAvailableForAllocation/ToPromise")])

P("CRM", GRP)

entity("Campaign", "C_Campaign", mono("key") + " = search key (Value)",
    [[mono("key"), "string", "Yes", "Value"],
     [mono("name"), "string", "Yes", "Name"],
     [mono("description"), "string", "-", "Description"],
     [mono("costs"), "number", "-", "Costs (defaults 0)"],
     [mono("startDate / endDate"), "string", "-", "StartDate / EndDate (ISO date)"],
     [mono("expectedRevenue / expectedLeads / actualRevenue / ..."), "number", "-", "campaign metrics"],
     [mono("channel / campaignType"), "ref", "-", "C_Channel by Name / C_CampaignType by Name"]])

entity("Lead", "C_Lead", mono("key") + " = your external reference (DocumentNo)",
    [[mono("key"), "string", "Yes", "DocumentNo (your external lead ref; used for dedupe)"],
     [mono("name"), "string", "Yes", "Name"],
     [mono("company / firstName / lastName / contactName / title"), "string", "-", "BPName / FirstName / LastName / ContactName / Title"],
     [mono("email / phone / mobile / url / description"), "string", "-", "EMail / Phone / Mobile / URL / Description"],
     [mono("numberEmployees / salesVolume"), "number", "-", "NumberEmployees / SalesVolume"],
     [mono("leadTarget"), "list", "-", "Customer | Partnership"],
     [mono("source / qualification"), "ref", "-", "R_Source by Name / C_LeadQualification by Name"],
     [mono("campaign / businessPartner"), "ref", "-", "C_Campaign by Value / C_BPartner by Value"],
     [mono("country / salesRegion"), "ref", "-", "C_Country by CountryCode / C_SalesRegion by Value"]],
    actions=[[mono("generateProspect"), "convert the lead into a business partner (prospect)"],
             [mono("generateOpportunity"), "create an opportunity from the lead"],
             [mono("createRequest"), "create a request/case from the lead"],
             [mono("addToTargetList"), "add the lead to a target list"]])

entity("Request", "R_Request", mono("key") + " = your external reference (DocumentNo)",
    [[mono("key"), "string", "Yes", "DocumentNo (your external ticket ref)"],
     [mono("summary"), "string", "Yes", "Summary (the request text)"],
     [mono("requestType"), "ref", "Yes", "R_RequestType by Name (Request | Question | Issue | Suggestion)"],
     [mono("priority"), "list", "-", "Urgent | High | Medium | Low | Minor"],
     [mono("requestAmt / comment"), "number / string", "-", "RequestAmt / Help"],
     [mono("status / category / source"), "ref", "-", "R_Status / R_Category / R_Source by Name"],
     [mono("businessPartner / campaign"), "ref", "-", "C_BPartner / C_Campaign by Value"]],
    children=[("updates[]", "R_RequestUpdate: result(req, the note text), confidentiality(Public|Partner Confidential|Internal), startTime, endTime")])

entity("Opportunity", "VAS_Opportunity", mono("key") + " = search key (Value)",
    [[mono("key"), "string", "Yes", "Value"],
     [mono("name"), "string", "Yes", "Name"],
     [mono("description / comments"), "string", "-", "Description / Comments"],
     [mono("probability / plannedAmt / estimatedBudget"), "number", "-", "Probability / PlannedAmt / VAS_EstimatedBudgetAmt"],
     [mono("enquiryDate / decisionDate / nextStepDate"), "string", "-", "ISO dates"],
     [mono("stage"), "list", "-", "Prospecting | Discovery/Design | Product Evaluation | Proposal | Negotiation | Won | Lost/Archived"],
     [mono("type"), "list", "-", "New Business | Renewal | Upsell | Cross-sell | Service Contract"],
     [mono("budget"), "list", "-", "Confirmed | Constrained | No Budget | Not Discussed | Planned"],
     [mono("riskLevel"), "list", "-", "Financial | Technical | Timeline | Resource | Competition | Approval Risk"],
     [mono("businessPartner / campaign"), "ref", "-", "C_BPartner / C_Campaign by Value"],
     [mono("currency"), "ref", "-", "C_Currency by ISO_Code"],
     [mono("lead"), "ref", "-", "C_Lead by external reference (DocumentNo)"]],
    children=[("lines[]", "VAS_OppLines: product(req, by Value), qty(req), price (used as sent), plannedAmt, discount, lineNo, description; uom derived from the product")],
    actions=[[mono("generateQuotation"), "create a sales quotation from the opportunity"],
             [mono("generateOrder"), "create a sales order from the opportunity"]],
    note="The C_Opportunity table is an unused stub; the real opportunity table is VAS_Opportunity (VAS module), with line children in VAS_OppLines.")

entity("Channel", "C_Channel", mono("key") + " = channel name",
    [[mono("key"), "string", "Yes", "Name (what Campaign 'channel' resolves)"]])

entity("CampaignType", "C_CampaignType", mono("key") + " = campaign type name",
    [[mono("key"), "string", "Yes", "Name (what Campaign 'campaignType' resolves)"]])

P("Sales", GRP)

entity("SalesQuotation", "C_Order", mono("key") + " = nominal (use poReference for your ref)",
    [[mono("key"), "string", "Yes", "DocumentNo (the model may reassign it; insert-only)"],
     [mono("businessPartner"), "ref", "Yes", "C_BPartner by Value"],
     [mono("docType"), "ref", "Yes", "C_DocType by Name ('Sales Quotation' / 'Sales Proposal')"],
     [mono("dateOrdered"), "string", "Yes", "DateOrdered (ISO date)"],
     [mono("priceList"), "ref", "Yes", "M_PriceList by Name"],
     [mono("currency"), "ref", "Yes", "C_Currency by ISO_Code"],
     [mono("warehouse"), "ref", "Yes", "M_Warehouse by Value (in the partner's organisation)"],
     [mono("paymentTerm"), "ref", "Yes", "C_PaymentTerm by Value"],
     [mono("poReference / description"), "string", "-", "POReference / Description"],
     [mono("organisation, bill-to location"), "derived", "-", "AD_Org_ID + C_BPartner_Location_ID from the partner (auto)"]],
    children=[("lines[]", "C_OrderLine: product(req, by Value), warehouse(req, by Value), qty(req), price(optional - if sent it is USED, else the price list drives), description. UOM + org are derived.")],
    actions=[[mono("complete"), "complete the document (DocAction CO)"],
             [mono("convertToOrder"), "convert the quotation to a sales order"]],
    note="Transactional document (header + lines). Created as a draft.")

P("HR / Payroll (Workday integration)", GRP)
P("The HR and payroll tables used when Onfinity is the payroll engine behind Workday: organisation "
  "structure (job families, jobs, grades, positions, holiday calendars) plus employee records "
  "(work info, statutory info, qualifications, dependents, bank accounts) and leave. The employee is "
  "a BusinessPartner - employee-linked entities reference it by its Value.", SMALL)

entity("JobFamily", "VA058_JobFamily", mono("key") + " = search key (Value)",
    [[mono("key"), "string", "Yes", "Value"],
     [mono("name"), "string", "Yes", "Name"],
     [mono("startDate"), "string", "Yes", "VA058_StartDate (ISO date)"]])

entity("Job", "VA058_Job", mono("key") + " = search key (Value)",
    [[mono("key"), "string", "Yes", "Value"],
     [mono("name"), "string", "Yes", "Name"],
     [mono("startDate"), "string", "Yes", "VA058_StartDate (ISO date)"],
     [mono("jobFamily"), "ref", "Yes", "VA058_JobFamily by Value"]])

entity("Department", "VA058_Department", mono("key") + " = search key (Value)",
    [[mono("key"), "string", "Yes", "Value"],
     [mono("name"), "string", "Yes", "Name"]])

entity("Grade", "VA058_Grade", mono("key") + " = search key (Value)",
    [[mono("key"), "string", "Yes", "Value"],
     [mono("name"), "string", "Yes", "Name"]])

entity("GradeRate", "VA058_GradeRate", mono("key") + " = search key (Value)",
    [[mono("key"), "string", "Yes", "Value"],
     [mono("name"), "string", "Yes", "Name"]],
    children=[("gradeValues[]", "VA058_GradeRtValue: grade(req, VA058_Grade by Value), minimum(req), midValue(req), maximum(req), userVal")])

entity("Position", "VA058_Position", mono("key") + " = search key (Value)",
    [[mono("key"), "string", "Yes", "Value"],
     [mono("name"), "string", "Yes", "Name"],
     [mono("startDate"), "string", "Yes", "VA058_StartDate (ISO date)"],
     [mono("job"), "ref", "Yes", "VA058_Job by Value"],
     [mono("grade"), "ref", "-", "VA058_Grade by Value"],
     [mono("qualification"), "ref", "-", "VA058_Qualification by Name"]],
    children=[("linkedGrades[]", "VA058_PosLinkedGrades: grade(req, VA058_Grade by Value)")])

entity("HolidayCalendar", "VA058_HolidayCalndr", mono("key") + " = calendar name",
    [[mono("key"), "string", "Yes", "VA058_HolidayCalendarName"],
     [mono("startDate / endDate"), "string", "-", "VA058_StartDate / VA058_EndDate (ISO date)"]])

entity("HolidayDay", "VA058_HolidayDay", mono("key") + " = holiday name (scoped by calendar)",
    [[mono("key"), "string", "Yes", "Name"],
     [mono("calendar"), "ref", "Yes", "VA058_HolidayCalndr by name"],
     [mono("date"), "string", "-", "Date1 (ISO date)"],
     [mono("hours"), "number", "-", "VA058_HolidayHrs"],
     [mono("hasRegion"), "boolean", "-", "VA058_HasRegion (default false)"]],
    children=[("regions[]", "VA058_HolidayCalndrRegion: country(C_Country by CountryCode)")],
    note="Holiday days belong to a calendar - pass 'calendar' to disambiguate the key.")

entity("Qualification", "VA058_Qualification", mono("key") + " = qualification name",
    [[mono("key"), "string", "Yes", "Name"],
     [mono("categoryName"), "list", "Yes", "Bachelor | Master | Honors Degree | Doctoral Degree | Phd"],
     [mono("country"), "ref", "Yes", "C_Country by CountryCode"]])

entity("University", "VA058_University", mono("key") + " = search key (Value)",
    [[mono("key"), "string", "Yes", "Value"],
     [mono("name"), "string", "Yes", "Name"],
     [mono("country"), "ref", "-", "C_Country by CountryCode"]])

entity("PartnerBankAccount", "C_BP_BankAccount", mono("accountNo") + " = account number (scoped by employee)",
    [[mono("accountNo"), "string", "Yes", "AccountNo (the key)"],
     [mono("businessPartner"), "ref", "Yes", "C_BPartner by Value (the employee/partner)"],
     [mono("bank"), "ref", "-", "C_Bank by Name"],
     [mono("accountName"), "string", "-", "A_Name"],
     [mono("bban"), "string", "-", "BBAN"],
     [mono("isACH"), "boolean", "-", "IsACH (default false)"]],
    note="Bank account of an employee/partner. Key = account number, scoped by businessPartner.")

entity("EmployeeWorkInfo", "VA058_EmpWorkInfo", mono("key") + " = assignment reference (scoped by employee)",
    [[mono("key"), "string", "Yes", "Value (your assignment reference)"],
     [mono("businessPartner"), "ref", "Yes", "C_BPartner by Value (the employee)"],
     [mono("startDate"), "string", "Yes", "VA058_StartDate (ISO date)"],
     [mono("primary"), "boolean", "Yes", "VA058_PrimaryFlag"],
     [mono("assignmentStatus"), "list", "Yes", "Confirmed | Probation | Suspended | Salary On Hold | Resignation | Retirement | Retrenchment | Terminate"],
     [mono("department / position / job / grade"), "ref", "-", "VA058_Department / VA058_Position / VA058_Job / VA058_Grade by Value"],
     [mono("endDate"), "string", "-", "VA058_EndDate (ISO date)"]])

entity("EmployeeStatutoryInfo", "VA096_StatInfo", mono("businessPartner") + " = the employee (key by reference)",
    [[mono("businessPartner"), "ref", "Yes", "C_BPartner by Value - IS the key (one record per employee)"],
     [mono("contributingToEPF"), "boolean", "-", "VA096_IsContributingToEPF"],
     [mono("uanNumber / epfNumber"), "string", "-", "VA096_UANNumber / VA096_EPFNumber"],
     [mono("contributingToTax"), "boolean", "-", "VA096_ContToTax"],
     [mono("incomeTaxNo"), "string", "-", "VA096_IncTaxNo"],
     [mono("contributingToESI"), "boolean", "-", "VA096_ControESI"],
     [mono("esiNumber"), "string", "-", "VA096_ESINum"],
     [mono("labourWelfareFund"), "string", "-", "VA096_LWF"]],
    note="India statutory info, 1:1 with an employee - keyed BY the employee. Resident-status and tax-regime lists are omitted pending code values.")

entity("EmployeeQualification", "VA058_EmpQualif", mono("key") + " = qualification name (scoped by employee)",
    [[mono("key"), "string", "Yes", "Name"],
     [mono("businessPartner"), "ref", "Yes", "C_BPartner by Value (the employee)"],
     [mono("qualification"), "ref", "-", "VA058_Qualification by Name"],
     [mono("university"), "ref", "-", "VA058_University by Value"]])

entity("EmployeeDependent", "VA058_EmpDependents", mono("key") + " = dependent name (scoped by employee)",
    [[mono("key"), "string", "Yes", "Name"],
     [mono("businessPartner"), "ref", "Yes", "C_BPartner by Value (the employee)"],
     [mono("relationType"), "list", "-", "Spouse | Child | Father | Mother | Brother | Sister | Grandfather | Grandmother | In-Law"],
     [mono("gender"), "list", "-", "Male | Female"],
     [mono("dateOfBirth"), "string", "-", "VA058_DOB (ISO date)"]],
    children=[("qualifications[]", "VA058_EmpDepQualif: name(req)")])

entity("LeaveType", "VA086_AbsenceType", mono("key") + " = leave type name",
    [[mono("key"), "string", "Yes", "VA086_Type"],
     [mono("startDate / endDate"), "string", "-", "VA086_StartDate / VA086_EndDate (ISO date)"],
     [mono("carriedOver"), "boolean", "-", "VA086_IsCarriedOver (default false)"],
     [mono("halfDayAllowed"), "boolean", "-", "VA086_IsHalfDay (default false)"],
     [mono("negativeAllowed"), "boolean", "-", "VA086_IsNegtiveLeave (default false)"],
     [mono("excludeHolidayWeekoff"), "boolean", "-", "VA086_IsExcludeHW (default false)"]],
    note="Category / units / balance lists are omitted pending code values.")

entity("LeaveEntry", "VA086_AbsenceEntries", mono("startDate") + " = start date (scoped by employee + leave type)",
    [[mono("startDate"), "string", "Yes", "VA086_StartDate (the key, ISO date)"],
     [mono("businessPartner"), "ref", "Yes", "C_BPartner by Value (the employee)"],
     [mono("absenceType"), "ref", "Yes", "VA086_AbsenceType by name"],
     [mono("endDate"), "string", "-", "VA086_EndDate (ISO date)"],
     [mono("duration"), "number", "-", "VA086_Duration (applied days)"],
     [mono("approvedDuration"), "number", "-", "VA086_ApprovedDuration"],
     [mono("comments"), "string", "-", "VA086_Comments"]],
    note="A leave record. Idempotency key = start date, scoped by employee + leave type.")

# ---------------------------------------------------------------- 10 quickstart
story.append(PageBreak())
P("10. Quick start (curl)", H1)
code('AK=... ; SK=... ; H="-H accessKey:$AK -H secretKey:$SK -H Content-Type:application/json"\n\n'
     '# discover\ncurl $H http://{host}:8090/v1/_entities\n\n'
     '# validate (no write)\n'
     'curl -X POST "http://{host}:8090/v1/Product?dryRun=true" $H \\\n'
     '  -d \'{"key":"SKU-100","name":"Widget","productType":"Item","category":"1000001","uom":"Each"}\'\n\n'
     '# create (PUT = upsert)\n'
     'curl -X PUT  http://{host}:8090/v1/Product $H \\\n'
     '  -d \'{"key":"SKU-100","name":"Widget","productType":"Item","category":"1000001","uom":"Each"}\'\n\n'
     '# read back\ncurl $H http://{host}:8090/v1/Product/SKU-100\n\n'
     '# run a process\ncurl -X POST http://{host}:8090/v1/Lead/LEAD-001/actions/generateProspect $H')

# ---------------------------------------------------------------- 11 AI rules
P("11. Integration rules for AI agents", H1)
table([
    ["Rule", "Detail"],
    ["Discover", "GET /v1/_entities ; GET /v1/{entity}/_actions ; /swagger/v1/swagger.json for the schema."],
    ["Authenticate", "Always send accessKey + secretKey. Without them every data call is 401."],
    ["Identifiers, not ids", "Send business identifiers for refs (country='DE'), never numeric *_ID values."],
    ["Validate first", "POST/PUT with ?dryRun=true and inspect 'resolved' before writing."],
    ["Upsert", "PUT to create-or-update by 'key'; idempotent, safe to retry."],
    ["Required fields", "Honour the 'Req' column in Section 9 on insert."],
    ["References / lists", "A ref must resolve to one active record; lists use the exact labels in Section 9."],
    ["Nested data", "Children as arrays; an owned record (e.g. address) as a nested object. Derived fields are not sent."],
    ["Actions", "POST /v1/{entity}/{key}/actions/{action}; pass process params in the body when required."],
    ["Errors", "Parse error.code (Section 8); 4xx = fix the payload, 502 = surface the Onfinity message."],
], [3.2*cm, 13.8*cm])
P("Flow: discover entity + required fields -> build payload with identifiers -> dry-run -> if 'resolved' "
  "is correct, repeat without dryRun -> read back / run an action. Treat /v1/_entities and /swagger as the "
  "live source of truth; the entity and action sets grow over time.", SMALL)

story.append(HRFlowable(width="100%", thickness=0.6, color=GRID, spaceBefore=6, spaceAfter=6))
P("Onfinity Smart API - Developer &amp; Integration Guide - v2.6 - 13 July 2026.", SMALL)

# ---------------------------------------------------------------- chrome
def footer(canvas, doc):
    canvas.saveState()
    canvas.setStrokeColor(GRID); canvas.setLineWidth(0.5)
    canvas.line(2*cm, 1.3*cm, A4[0]-2*cm, 1.3*cm)
    canvas.setFont("Helvetica", 7.5); canvas.setFillColor(GREY)
    canvas.drawString(2*cm, 1.0*cm, "Onfinity Smart API - Developer & Integration Guide  v2.6")
    canvas.drawRightString(A4[0]-2*cm, 1.0*cm, "Page %d" % doc.page)
    canvas.restoreState()

# TOC needs two passes (multiBuild). Capture H1/GRP/ENT headings into the TOC.
class TOCDoc(BaseDocTemplate):
    def afterFlowable(self, flowable):
        if isinstance(flowable, Paragraph):
            name = flowable.style.name
            level = {"H1": 0, "GRP": 1, "ENT": 2}.get(name)
            if level is not None:
                self.notify("TOCEntry", (level, flowable.getPlainText(), self.page))

doc = TOCDoc(OUT, pagesize=A4, leftMargin=2*cm, rightMargin=2*cm,
             topMargin=1.7*cm, bottomMargin=1.7*cm,
             title="Onfinity Smart API - Developer & Integration Guide",
             author="Vienna Advantage / Onfinity")
frame = Frame(doc.leftMargin, doc.bottomMargin, doc.width, doc.height, id="main")
doc.addPageTemplates([PageTemplate(id="main", frames=[frame], onPage=footer)])
doc.multiBuild(story)
print("WROTE", OUT)
