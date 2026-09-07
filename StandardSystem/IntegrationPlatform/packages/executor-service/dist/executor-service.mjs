// src/server.ts
import { createServer } from "node:http";

// ../universal-executor/src/template.ts
var EXPR = /\{\{\s*([^}]+?)\s*\}\}/g;
var WHOLE = /^\{\{\s*([^}]+?)\s*\}\}$/;
function getPath(ctx, path) {
  const parts = [];
  for (const seg of path.split(".")) {
    const m = seg.matchAll(/([^[\]]+)|\[(-?\d+)\]/g);
    for (const g of m) {
      if (g[2] !== void 0) parts.push(Number(g[2]));
      else if (g[1] !== void 0) parts.push(g[1]);
    }
  }
  let cur = ctx;
  for (const p of parts) {
    if (cur == null) return void 0;
    if (typeof p === "number" && Array.isArray(cur)) {
      cur = p < 0 ? cur[cur.length + p] : cur[p];
    } else if (typeof cur === "object") {
      cur = cur[String(p)];
    } else {
      return void 0;
    }
  }
  return cur;
}
function renderValue(template, ctx) {
  const whole = template.match(WHOLE);
  if (whole) return getPath(ctx, whole[1]);
  return template.replace(EXPR, (_, expr) => {
    const v = getPath(ctx, expr.trim());
    return v == null ? "" : typeof v === "object" ? JSON.stringify(v) : String(v);
  });
}
function render(node, ctx) {
  if (typeof node === "string") return renderValue(node, ctx);
  if (Array.isArray(node)) return node.map((n) => render(n, ctx));
  if (node && typeof node === "object") {
    const out = {};
    for (const [k, v] of Object.entries(node)) out[k] = render(v, ctx);
    return out;
  }
  return node;
}
function renderStringMap(map, ctx) {
  const out = {};
  if (!map) return out;
  for (const [k, tpl] of Object.entries(map)) {
    const v = renderValue(tpl, ctx);
    if (v == null || v === "") continue;
    out[k] = typeof v === "object" ? JSON.stringify(v) : String(v);
  }
  return out;
}

// ../universal-executor/src/auth.ts
function b64(s) {
  return Buffer.from(s, "utf8").toString("base64");
}
function applyAuth(auth, ctx) {
  const headers = {};
  const query = {};
  const fields = ctx.auth?.fields ?? {};
  switch (auth.type) {
    case "apiKey": {
      if (!auth.apiKey) break;
      const value = String(renderValue(auth.apiKey.valueTemplate, ctx) ?? "");
      if (auth.apiKey.in === "header") headers[auth.apiKey.name] = value;
      else query[auth.apiKey.name] = value;
      break;
    }
    case "basic": {
      if (!auth.basic) break;
      const u = String(fields[auth.basic.usernameField] ?? "");
      const p = String(fields[auth.basic.passwordField] ?? "");
      headers["Authorization"] = `Basic ${b64(`${u}:${p}`)}`;
      break;
    }
    case "oauth2": {
      const tpl = auth.oauth2?.accessTokenTemplate ?? "Bearer {{auth.tokens.access_token}}";
      headers["Authorization"] = String(renderValue(tpl, ctx) ?? "");
      break;
    }
    case "bearer": {
      const token = fields.token ?? fields.api_key;
      if (token) headers["Authorization"] = `Bearer ${token}`;
      break;
    }
    case "none":
    case "custom":
    default:
      break;
  }
  return { headers, query };
}

// ../universal-executor/src/request.ts
function joinUrl(base, path) {
  return `${base.replace(/\/+$/, "")}/${path.replace(/^\/+/, "")}`;
}
function toQueryString(q) {
  const s = new URLSearchParams(q).toString();
  return s ? `?${s}` : "";
}
function buildRequest(manifest, req, ctx, extraQuery = {}) {
  const baseUrl = String(renderValue(manifest.connection.baseUrl, ctx) ?? "");
  const path = String(renderValue(req.path, ctx) ?? "");
  const auth = applyAuth(manifest.auth, ctx);
  const headers = {
    ...renderStringMap(manifest.connection.defaultHeaders, ctx),
    ...renderStringMap(req.headers, ctx),
    ...auth.headers
  };
  const query = {
    ...renderStringMap(req.query, ctx),
    ...auth.query,
    ...extraQuery
    // pagination params override
  };
  let body;
  const bodyType = req.bodyType ?? "json";
  if (req.body != null && bodyType !== "none") {
    const rendered = render(req.body, ctx);
    if (bodyType === "json") {
      body = typeof rendered === "string" ? rendered : JSON.stringify(rendered);
      headers["Content-Type"] ??= "application/json";
    } else if (bodyType === "form") {
      const flat = {};
      for (const [k, v] of Object.entries(rendered)) {
        if (v == null) continue;
        flat[k] = typeof v === "object" ? JSON.stringify(v) : String(v);
      }
      body = new URLSearchParams(flat).toString();
      headers["Content-Type"] ??= "application/x-www-form-urlencoded";
    }
  }
  return { url: joinUrl(baseUrl, path) + toQueryString(query), method: req.method, headers, body };
}
async function sendRequest(built) {
  const res = await fetch(built.url, {
    method: built.method,
    headers: built.headers,
    body: built.body
  });
  const text = await res.text();
  let parsed = text;
  try {
    parsed = text ? JSON.parse(text) : null;
  } catch {
  }
  const headers = {};
  res.headers.forEach((v, k) => headers[k] = v);
  return { status: res.status, ok: res.ok, headers, body: parsed };
}

// ../universal-executor/src/pagination.ts
function resolvePagination(manifest, pag) {
  if (!pag) return void 0;
  if ("$use" in pag) return manifest.paginators?.[pag.$use];
  return pag;
}
var itemsAt = (res, path) => {
  const v = getPath(res.body, path);
  return Array.isArray(v) ? v : [];
};
async function paginate(manifest, req, pag, ctx) {
  const items = [];
  const size = pag.pageSize ?? 100;
  const maxPages = pag.maxPages ?? 50;
  let pages = 0;
  let cursor;
  let page = 1;
  let offset = 0;
  while (pages < maxPages) {
    const extra = {};
    if (pag.limitParam) extra[pag.limitParam] = String(size);
    if (pag.type === "cursor" && cursor && pag.cursorRequestParam) extra[pag.cursorRequestParam] = cursor;
    if (pag.type === "page" && pag.pageParam) extra[pag.pageParam] = String(page);
    if (pag.type === "offset" && pag.offsetParam) extra[pag.offsetParam] = String(offset);
    const res = await sendRequest(buildRequest(manifest, req, ctx, extra));
    pages++;
    const batch = itemsAt(res, pag.itemsPath);
    items.push(...batch);
    const hasMoreFlag = pag.hasMorePath ? Boolean(getPath(res.body, pag.hasMorePath)) : void 0;
    if (pag.type === "cursor") {
      const next = pag.cursorResponsePath ? getPath(res.body, pag.cursorResponsePath) : void 0;
      cursor = next == null ? void 0 : String(next);
      if (!cursor || hasMoreFlag === false || batch.length === 0) break;
    } else if (pag.type === "page") {
      page++;
      if (hasMoreFlag === false || batch.length < size || batch.length === 0) break;
    } else if (pag.type === "offset") {
      offset += size;
      if (hasMoreFlag === false || batch.length < size || batch.length === 0) break;
    } else {
      break;
    }
  }
  return { items, pages, truncated: pages >= maxPages };
}

// ../universal-executor/src/index.ts
var sleep = (ms) => new Promise((r) => setTimeout(r, ms));
function matchError(rules, status) {
  return rules?.find((r) => r.whenStatus?.includes(status));
}
async function sendWithRetry(built, rate) {
  const retryOn = new Set(rate?.retryOn ?? [429, 503]);
  const maxRetries = rate?.maxRetries ?? 5;
  let attempt = 0;
  for (; ; ) {
    const res = await sendRequest(built);
    if (!retryOn.has(res.status) || attempt >= maxRetries) return res;
    const retryAfter = rate?.respectRetryAfterHeader !== false ? Number(res.headers["retry-after"]) : NaN;
    const waitMs = Number.isFinite(retryAfter) ? retryAfter * 1e3 : Math.min(2 ** attempt * 250, 8e3);
    await sleep(waitMs);
    attempt++;
  }
}
function buildContext(manifest, cred, input) {
  return {
    input: input ?? {},
    auth: { fields: cred.fields ?? {}, tokens: cred.tokens ?? {} },
    connection: cred.connection ?? {}
  };
}
async function executeAction(manifest, actionKey, input, cred) {
  const action = manifest.actions?.[actionKey];
  if (!action) throw new Error(`unknown action: ${manifest.key}.${actionKey}`);
  const ctx = buildContext(manifest, cred, input);
  const rate = action.rateLimit ?? manifest.rateLimit;
  const pag = resolvePagination(manifest, action.pagination);
  if (pag && pag.type !== "none") {
    const result = await paginate(manifest, action.request, pag, ctx);
    return { outcome: "success", data: result.items, pages: result.pages, truncated: result.truncated };
  }
  const res = await sendWithRetry(buildRequest(manifest, action.request, ctx), rate);
  if (res.ok) return { outcome: "success", status: res.status, data: res.body };
  const rule = matchError(action.errors, res.status);
  const message = rule?.messagePath ? String(getPath(res.body, rule.messagePath) ?? "") : void 0;
  return { outcome: rule?.outcome ?? "fail", status: res.status, data: res.body, message };
}

// ../../connectors/stripe.connector.json
var stripe_connector_default = {
  $schema: "../schema/connector-manifest.schema.json",
  manifestVersion: "1.0",
  key: "stripe",
  name: "Stripe",
  version: "0.1.0",
  description: "Payments, customers, and subscriptions. Reference ecommerce/finance connector generated from Stripe's OpenAPI spec.",
  vendor: "Stripe, Inc.",
  categories: ["ecommerce", "finance"],
  homepage: "https://stripe.com",
  docsUrl: "https://docs.stripe.com/api",
  source: {
    type: "openapi",
    specUrl: "https://raw.githubusercontent.com/stripe/openapi/master/openapi/spec3.json",
    specVersion: "3.0.0",
    generatedBy: "onfinity-connector-factory@1",
    reviewed: false
  },
  connection: {
    baseUrl: "https://api.stripe.com",
    defaultHeaders: {
      "Stripe-Version": "2024-06-20"
    }
  },
  auth: {
    type: "apiKey",
    fields: [
      {
        id: "secret_key",
        label: "Secret API key",
        type: "password",
        secret: true,
        required: true,
        help: "Found in Stripe Dashboard \u2192 Developers \u2192 API keys. Begins with sk_live_ or sk_test_.",
        placeholder: "sk_live_..."
      }
    ],
    apiKey: {
      in: "header",
      name: "Authorization",
      valueTemplate: "Bearer {{auth.fields.secret_key}}"
    },
    test: {
      action: "list_customers",
      input: { limit: 1 }
    }
  },
  rateLimit: {
    requests: 90,
    perSeconds: 1,
    retryOn: [429, 503],
    maxRetries: 5,
    respectRetryAfterHeader: true
  },
  paginators: {
    stripe_cursor: {
      type: "cursor",
      itemsPath: "data",
      cursorRequestParam: "starting_after",
      cursorResponsePath: "data[-1].id",
      hasMorePath: "has_more",
      limitParam: "limit",
      pageSize: 100,
      maxPages: 50
    }
  },
  actions: {
    create_customer: {
      title: "Create customer",
      description: "Create a new Stripe customer.",
      request: {
        method: "POST",
        path: "/v1/customers",
        bodyType: "form",
        body: {
          email: "{{input.email}}",
          name: "{{input.name}}",
          description: "{{input.description}}"
        }
      },
      input: {
        type: "object",
        required: ["email"],
        properties: {
          email: { type: "string", format: "email" },
          name: { type: "string" },
          description: { type: "string" }
        }
      },
      output: {
        type: "object",
        properties: {
          id: { type: "string" },
          email: { type: "string" },
          name: { type: "string" },
          created: { type: "integer" }
        }
      },
      errors: [
        { whenStatus: [400, 404], outcome: "fail", messagePath: "error.message" },
        { whenStatus: [401], outcome: "reauth", messagePath: "error.message" },
        { whenStatus: [429], outcome: "retry" }
      ],
      sample: {
        input: { email: "ada@example.com", name: "Ada Lovelace" },
        output: { id: "cus_Nffr...", email: "ada@example.com", name: "Ada Lovelace", created: 17189e5 }
      }
    },
    list_customers: {
      title: "List customers",
      description: "List customers, newest first, with automatic cursor pagination.",
      request: {
        method: "GET",
        path: "/v1/customers",
        query: {
          limit: "{{input.limit}}",
          email: "{{input.email}}"
        }
      },
      input: {
        type: "object",
        properties: {
          limit: { type: "integer", default: 100, maximum: 100 },
          email: { type: "string", description: "Optional exact-match email filter." }
        }
      },
      output: {
        type: "object",
        properties: {
          data: { type: "array", items: { type: "object" } },
          has_more: { type: "boolean" }
        }
      },
      pagination: { $use: "stripe_cursor" }
    }
  },
  triggers: {
    payment_succeeded: {
      title: "Payment succeeded",
      description: "Fires when a PaymentIntent succeeds. Delivered in real time via Stripe webhooks.",
      type: "webhook",
      input: {
        type: "object",
        properties: {
          events: {
            type: "array",
            items: { type: "string" },
            default: ["payment_intent.succeeded"]
          }
        }
      },
      output: {
        type: "object",
        properties: {
          id: { type: "string" },
          type: { type: "string" },
          data: { type: "object" }
        }
      },
      dedupeKeyPath: "id",
      webhook: {
        subscribe: {
          method: "POST",
          path: "/v1/webhook_endpoints",
          bodyType: "form",
          body: {
            url: "{{webhook.url}}",
            "enabled_events[]": "{{input.events}}"
          }
        },
        unsubscribe: {
          method: "DELETE",
          path: "/v1/webhook_endpoints/{{state.webhook_id}}"
        },
        eventTypePath: "type",
        verification: {
          type: "signature_header",
          header: "Stripe-Signature",
          algorithm: "sha256",
          secretField: "webhook_signing_secret"
        }
      },
      sample: {
        id: "evt_1P...",
        type: "payment_intent.succeeded",
        data: { object: { id: "pi_3P...", amount: 4200, currency: "usd" } }
      }
    },
    new_charge_poll: {
      title: "New charge (polling)",
      description: "Fallback trigger for environments where webhooks can't be received \u2014 polls for charges created since the last run.",
      type: "polling",
      output: {
        type: "object",
        properties: {
          id: { type: "string" },
          amount: { type: "integer" },
          currency: { type: "string" },
          created: { type: "integer" }
        }
      },
      dedupeKeyPath: "id",
      polling: {
        request: {
          method: "GET",
          path: "/v1/charges",
          query: {
            "created[gte]": "{{cursor.value}}",
            limit: "100"
          }
        },
        itemsPath: "data",
        intervalSeconds: 300,
        cursor: {
          type: "timestamp",
          requestParam: "created[gte]",
          responsePath: "created"
        }
      }
    }
  }
};

// ../../connectors/onfinity.connector.json
var onfinity_connector_default = {
  $schema: "../schema/connector-manifest.schema.json",
  manifestVersion: "1.0",
  key: "onfinity",
  name: "Onfinity",
  version: "0.1.0",
  description: "Onfinity (ViennaAdvantage) ERP/CRM via VAAPI. The platform's home hub: insert records, run processes/jobs (workflows), read data, and (provisionally) invoke AI agents. Onfinity's outbound event calls are modeled as a webhook trigger. Built from VAAPI Services doc v2.0.",
  vendor: "ViennaAdvantage / Onfinity",
  categories: ["erp", "crm"],
  docsUrl: "https://viennaadvantage.atlassian.net/wiki/spaces/VA",
  source: {
    type: "manual",
    specVersion: "VAAPI Services v2.0",
    generatedBy: "hand-authored from VAAPI Services Document v2.0",
    reviewed: false
  },
  connection: {
    baseUrl: "{{connection.base_url}}",
    variables: [
      {
        id: "base_url",
        label: "Onfinity base URL",
        type: "url",
        required: true,
        help: "Your Onfinity instance root, replacing the doc's https://OnfinitySample.com. No trailing /api.",
        placeholder: "https://erp.yourcompany.com"
      },
      {
        id: "ad_client_id",
        label: "Client ID (AD_Client_ID)",
        type: "number",
        required: true,
        help: "Tenant/client id used when building insert SQL. From GetClients."
      },
      {
        id: "ad_org_id",
        label: "Organization ID (AD_Org_ID)",
        type: "number",
        required: true,
        help: "Org id used when building insert SQL. From GetOrgs."
      },
      {
        id: "ad_role_id",
        label: "Role ID (AD_Role_ID)",
        type: "number",
        required: false,
        help: "Role the integration acts as. From Login."
      },
      {
        id: "ad_user_id",
        label: "User ID (AD_User_ID)",
        type: "number",
        required: false,
        help: "User the integration acts as. From Login."
      }
    ],
    defaultHeaders: {
      "Content-Type": "application/json",
      accessKey: "{{auth.fields.access_key}}",
      secretKey: "{{auth.fields.secret_key}}"
    }
  },
  auth: {
    type: "custom",
    fields: [
      {
        id: "access_key",
        label: "Access key",
        type: "password",
        secret: true,
        required: true,
        help: "Provided by Onfinity Support. Sent as the accessKey header."
      },
      {
        id: "secret_key",
        label: "Secret key",
        type: "password",
        secret: true,
        required: true,
        help: "API Secret Key created in Onfinity admin (Creating an API Secret Key). Sent as the secretKey header. If your deployment requires a session token instead, mint one with the get_api_token action and use it here."
      }
    ],
    test: {
      action: "get_record",
      input: { tableName: "AD_System", selectQuery: "SELECT Name FROM AD_System" }
    }
  },
  actions: {
    insert_record: {
      title: "Insert record",
      description: "Insert a row via an INSERT SQL statement. Use @pk where the primary key goes; Onfinity generates it and returns it. POST /Service/InsertRecord.",
      request: {
        method: "POST",
        path: "/api/VAAPI/Service/InsertRecord",
        bodyType: "json",
        body: {
          tableName: "{{input.tableName}}",
          selectQuery: "{{input.selectQuery}}"
        }
      },
      input: {
        type: "object",
        required: ["tableName", "selectQuery"],
        properties: {
          tableName: { type: "string", description: "Target table, e.g. C_BPartner." },
          selectQuery: {
            type: "string",
            description: "INSERT statement. Include @pk for the primary key and reference {{connection.ad_client_id}} / {{connection.ad_org_id}} for tenant context."
          }
        }
      },
      output: {
        type: "object",
        properties: {
          id: { type: "integer", description: "Generated primary key." },
          identifier: { type: "string" },
          code: { type: "integer" },
          result: { type: "string" },
          data: { type: "integer" }
        }
      },
      errors: [
        { whenStatus: [401, 403], outcome: "reauth", messagePath: "result" },
        { whenStatus: [400, 500], outcome: "fail", messagePath: "result" }
      ],
      sample: {
        input: {
          tableName: "CM_ChatEntry",
          selectQuery: "INSERT INTO CM_ChatEntry (AD_Client_ID, AD_Org_ID, AD_User_ID, CM_ChatEntry_ID, CM_Chat_ID, CharacterData, CreatedBy, UpdatedBy, IsActive, ConfidentialType) VALUES (1000005,1000008,1005338,@pk,1000033,'Hello',1005338,1005338,'Y','A')"
        },
        output: { id: 1000324, identifier: "1000324", code: 200, result: "Success", data: 1000324 }
      }
    },
    get_record: {
      title: "Get records",
      description: "Read rows with a SELECT query. POST /Service/GetRecordV2 (POST variant, body-safe).",
      request: {
        method: "POST",
        path: "/api/VAAPI/Service/GetRecordV2",
        bodyType: "json",
        body: {
          tableName: "{{input.tableName}}",
          selectQuery: "{{input.selectQuery}}"
        }
      },
      input: {
        type: "object",
        required: ["tableName", "selectQuery"],
        properties: {
          tableName: { type: "string" },
          selectQuery: { type: "string", description: "SELECT statement." }
        }
      },
      output: {
        type: "object",
        properties: {
          code: { type: "integer" },
          result: { type: "string" },
          data: { type: "array", items: { type: "object" } }
        }
      },
      sample: {
        input: { tableName: "C_BPartner", selectQuery: "SELECT Name, Value FROM C_BPartner" },
        output: { code: 200, result: "Success", data: [{ name: "Johan Martin", value: "JohanMartin" }] }
      }
    },
    run_process: {
      title: "Run process (workflow/job)",
      description: "Execute an Onfinity job/process by AD_Process_ID. This is how a workflow triggers Onfinity logic. POST /Service/RunProcess.",
      request: {
        method: "POST",
        path: "/api/VAAPI/Service/RunProcess",
        bodyType: "json",
        body: {
          AD_Process_ID: "{{input.process_id}}",
          Record_ID: "{{input.record_id}}",
          Param: "{{input.params}}"
        }
      },
      input: {
        type: "object",
        required: ["process_id"],
        properties: {
          process_id: { type: "integer", description: "AD_Process_ID provided by Onfinity for the validated job." },
          record_id: { type: "integer", default: 0, description: "0 for a menu job; otherwise the record to run against." },
          params: {
            type: "array",
            description: "Job parameters as Name/Value pairs.",
            items: { type: "object", required: ["Name", "Value"], properties: { Name: { type: "string" }, Value: {} } }
          }
        }
      },
      output: {
        type: "object",
        properties: { code: { type: "integer" }, result: { type: "string" }, data: { type: "string" } }
      },
      sample: {
        input: { process_id: 1000733, record_id: 1000798, params: [{ Name: "C_BPartner_ID", Value: 1007094 }] },
        output: { code: 200, result: "Success", data: "Process executed successfully" }
      }
    },
    run_process_by_search_key: {
      title: "Run process by search key",
      description: "Same as run_process but addresses the job by its portable SearchKey instead of a per-install AD_Process_ID. Prefer this for connectors shared across installs. POST /Service/RunProcessBySearchKey.",
      request: {
        method: "POST",
        path: "/api/VAAPI/Service/RunProcessBySearchKey",
        bodyType: "json",
        body: {
          SearchKey: "{{input.search_key}}",
          Record_ID: "{{input.record_id}}",
          Param: "{{input.params}}"
        }
      },
      input: {
        type: "object",
        required: ["search_key"],
        properties: {
          search_key: { type: "string", description: "Job search key from the Job & Report screen, e.g. VA075_UpdateFSRStatus." },
          record_id: { type: "integer", default: 0 },
          params: { type: "array", items: { type: "object", properties: { Name: { type: "string" }, Value: {} } } }
        }
      },
      output: {
        type: "object",
        properties: { code: { type: "integer" }, result: { type: "string" }, data: { type: "string" } }
      },
      sample: {
        input: { search_key: "VA075_UpdateFSRStatus", record_id: 1000031, params: [{ Name: "TaskStatus", Value: "09" }] },
        output: { code: 200, result: "Success", data: "Process executed successfully" }
      }
    },
    call_ai_agent: {
      title: "Call AI agent (provisional)",
      description: "PROVISIONAL \u2014 VAAPI v2.0 exposes no dedicated AI-agent endpoint. Modeled as running the agent's orchestration job by search key. Replace with the real Onfinity AI runtime endpoint (VAI01) once available. See the ai-orchestration notes.",
      request: {
        method: "POST",
        path: "/api/VAAPI/Service/RunProcessBySearchKey",
        bodyType: "json",
        body: {
          SearchKey: "{{input.agent_search_key}}",
          Record_ID: "{{input.record_id}}",
          Param: "{{input.params}}"
        }
      },
      input: {
        type: "object",
        required: ["agent_search_key"],
        properties: {
          agent_search_key: { type: "string", description: "Search key of the agent's orchestration job/blueprint." },
          record_id: { type: "integer", default: 0 },
          params: {
            type: "array",
            description: "Agent inputs as Name/Value pairs (e.g. a prompt or context record).",
            items: { type: "object", properties: { Name: { type: "string" }, Value: {} } }
          }
        }
      },
      output: {
        type: "object",
        properties: { code: { type: "integer" }, result: { type: "string" }, data: {} }
      }
    },
    get_api_token: {
      title: "Get API token",
      description: "Mint a time-boxed API token (used as secret_key on subsequent calls if your deployment requires a session token rather than the static secret key). POST /Service/GetApiToken.",
      request: {
        method: "POST",
        path: "/api/VAAPI/Service/GetApiToken",
        bodyType: "json",
        body: {
          AD_CLIENT_ID: "{{connection.ad_client_id}}",
          AD_ORG_ID: "{{connection.ad_org_id}}",
          AD_Role_ID: "{{connection.ad_role_id}}",
          AD_User_ID: "{{connection.ad_user_id}}",
          TokenExpireTimeInMinutes: "{{input.expire_minutes}}",
          Password: "{{input.password}}"
        }
      },
      input: {
        type: "object",
        required: ["password"],
        properties: {
          password: { type: "string", description: "Onfinity user password." },
          expire_minutes: { type: "integer", default: 1440 }
        }
      },
      output: {
        type: "object",
        properties: {
          code: { type: "integer" },
          result: { type: "string" },
          data: { type: "object", properties: { token: { type: "string" }, tokenExpireOn: { type: "string" } } }
        }
      }
    }
  },
  triggers: {
    record_event: {
      title: "Onfinity record event (outbound)",
      description: "Fires when Onfinity emits an outbound event call (record created/updated/deleted, or a process completing). Onfinity is configured to POST events to this trigger's delivery URL; there is no VAAPI register endpoint, so subscription is set up inside Onfinity pointing at {{webhook.url}}.",
      type: "webhook",
      input: {
        type: "object",
        properties: {
          tableName: { type: "string", description: "Optional filter \u2014 only emit for this table, e.g. C_BPartner." },
          events: {
            type: "array",
            items: { type: "string", enum: ["created", "updated", "deleted", "process_completed"] },
            default: ["created", "updated"]
          }
        }
      },
      output: {
        type: "object",
        properties: {
          event: { type: "string", description: "created | updated | deleted | process_completed." },
          tableName: { type: "string" },
          AD_Table_ID: { type: "integer" },
          Record_ID: { type: "integer" },
          AD_Client_ID: { type: "integer" },
          AD_Org_ID: { type: "integer" },
          data: { type: "object", description: "Changed record payload as JSON." }
        }
      },
      dedupeKeyPath: "Record_ID",
      webhook: {
        eventTypePath: "event",
        verification: {
          type: "hmac",
          header: "X-Onfinity-Signature",
          algorithm: "sha256",
          secretField: "webhook_signing_secret"
        }
      },
      sample: {
        event: "created",
        tableName: "C_BPartner",
        AD_Table_ID: 291,
        Record_ID: 1007094,
        AD_Client_ID: 1000005,
        AD_Org_ID: 1000008,
        data: { Name: "Johan Martin", Value: "JohanMartin" }
      }
    }
  }
};

// src/server.ts
var CATALOG = {
  [stripe_connector_default.key]: stripe_connector_default,
  [onfinity_connector_default.key]: onfinity_connector_default
};
var PORT = Number(process.env.EXECUTOR_PORT ?? 3070);
var HOST = process.env.EXECUTOR_HOST ?? "0.0.0.0";
function send(res, status, body) {
  const json = JSON.stringify(body);
  res.writeHead(status, { "content-type": "application/json", "content-length": Buffer.byteLength(json) });
  res.end(json);
}
function readBody(req) {
  return new Promise((resolve, reject) => {
    let data = "";
    req.on("data", (c) => data += c);
    req.on("end", () => {
      try {
        resolve(data ? JSON.parse(data) : {});
      } catch (e) {
        reject(e);
      }
    });
    req.on("error", reject);
  });
}
function catalogSummary() {
  return Object.values(CATALOG).map((m) => ({
    key: m.key,
    name: m.name,
    categories: m.categories ?? [],
    actions: Object.keys(m.actions ?? {}),
    triggers: Object.keys(m.triggers ?? {})
  }));
}
var server = createServer(async (req, res) => {
  try {
    const url = new URL(req.url ?? "/", `http://${req.headers.host}`);
    if (req.method === "GET" && url.pathname === "/health") return send(res, 200, { ok: true, connectors: Object.keys(CATALOG) });
    if (req.method === "GET" && url.pathname === "/catalog") return send(res, 200, catalogSummary());
    if (req.method === "POST" && url.pathname === "/run") {
      const body = await readBody(req);
      const { connector, action, input, credential } = body ?? {};
      const manifest = CATALOG[connector];
      if (!manifest) return send(res, 404, { error: `unknown connector: ${connector}`, available: Object.keys(CATALOG) });
      if (!action) return send(res, 400, { error: "missing 'action'" });
      const cred = credential ?? { fields: {} };
      const result = await executeAction(manifest, action, input ?? {}, cred);
      return send(res, 200, result);
    }
    return send(res, 404, { error: "not found", routes: ["GET /health", "GET /catalog", "POST /run"] });
  } catch (e) {
    return send(res, 500, { error: String(e?.message ?? e) });
  }
});
server.listen(PORT, HOST, () => {
  console.log(`executor-service listening on ${HOST}:${PORT} \u2014 connectors: ${Object.keys(CATALOG).join(", ")}`);
});
