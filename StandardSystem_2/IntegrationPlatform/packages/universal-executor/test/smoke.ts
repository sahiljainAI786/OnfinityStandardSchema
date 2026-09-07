// Local smoke test for the universal executor. Run: npx tsx test/smoke.ts
// Covers: template renderer, auth/header injection (no network), a live HTTP action, and pagination.

import { getPath, renderValue, render } from "../src/template.js";
import { buildRequest } from "../src/request.js";
import { executeAction, type Credential } from "../src/index.js";
import type { ConnectorManifest } from "@ip/shared";

let pass = 0;
let fail = 0;
function check(name: string, cond: boolean, extra = "") {
  if (cond) { pass++; console.log("  ok  -", name); }
  else { fail++; console.log("  FAIL-", name, extra); }
}
const eq = (a: unknown, b: unknown) => JSON.stringify(a) === JSON.stringify(b);

async function main() {
  // --- A. template renderer (pure) ---
  console.log("A. template renderer");
  check("dotted path", getPath({ a: { b: 5 } }, "a.b") === 5);
  check("array index", getPath({ d: [{ id: "x" }, { id: "y" }] }, "d[1].id") === "y");
  check("negative index", getPath({ d: [1, 2, 3] }, "d[-1]") === 3);
  check("whole-string keeps type (array)", eq(renderValue("{{p}}", { p: [1, 2] }), [1, 2]));
  check("embedded coerces to string", renderValue("id-{{n}}", { n: 7 }) === "id-7");
  check("missing → empty", renderValue("x{{nope}}y", {}) === "xy");
  check(
    "object render",
    eq(render({ a: "{{x}}", b: "k-{{x}}" }, { x: 3 }), { a: 3, b: "k-3" }),
  );

  // --- B. auth + header injection (no network) ---
  console.log("B. auth / header injection");
  const apiKeyManifest = {
    manifestVersion: "1.0", key: "t", name: "T",
    connection: { baseUrl: "https://api.example.com", defaultHeaders: { "X-Trace": "1" } },
    auth: { type: "apiKey", apiKey: { in: "header", name: "Authorization", valueTemplate: "Bearer {{auth.fields.api_key}}" } },
  } as unknown as ConnectorManifest;
  const reqA = buildRequest(apiKeyManifest, { method: "GET", path: "/v1/x", query: { q: "{{input.q}}" } },
    { input: { q: "hi" }, auth: { fields: { api_key: "SECRET" } }, connection: {} });
  check("apiKey header injected", reqA.headers["Authorization"] === "Bearer SECRET");
  check("defaultHeader merged", reqA.headers["X-Trace"] === "1");
  check("query rendered + appended", reqA.url === "https://api.example.com/v1/x?q=hi", reqA.url);

  // multi-header custom auth (the Onfinity pattern: two keys via defaultHeaders)
  const onfMan = {
    manifestVersion: "1.0", key: "onf", name: "Onf",
    connection: { baseUrl: "{{connection.base_url}}", defaultHeaders: { accessKey: "{{auth.fields.access_key}}", secretKey: "{{auth.fields.secret_key}}" } },
    auth: { type: "custom" },
  } as unknown as ConnectorManifest;
  const reqO = buildRequest(onfMan, { method: "POST", path: "/api/VAAPI/Service/GetRecordV2", bodyType: "json", body: { tableName: "{{input.t}}" } },
    { input: { t: "C_BPartner" }, auth: { fields: { access_key: "AK", secret_key: "SK" } }, connection: { base_url: "https://erp.acme.com" } });
  check("custom multi-header: accessKey", reqO.headers["accessKey"] === "AK");
  check("custom multi-header: secretKey", reqO.headers["secretKey"] === "SK");
  check("templated baseUrl resolved", reqO.url === "https://erp.acme.com/api/VAAPI/Service/GetRecordV2", reqO.url);
  check("json body rendered", reqO.body === JSON.stringify({ tableName: "C_BPartner" }));

  // --- C. live HTTP action (public no-auth API) ---
  console.log("C. live HTTP (jsonplaceholder)");
  const jp = {
    manifestVersion: "1.0", key: "jp", name: "JSONPlaceholder",
    connection: { baseUrl: "https://jsonplaceholder.typicode.com" },
    auth: { type: "none" },
    actions: {
      get_todo: { title: "Get todo", request: { method: "GET", path: "/todos/{{input.id}}" } },
      create_post: { title: "Create post", request: { method: "POST", path: "/posts", bodyType: "json", body: { title: "{{input.title}}", userId: "{{input.userId}}" } } },
    },
  } as unknown as ConnectorManifest;
  const noCred: Credential = { fields: {} };
  try {
    const got = await executeAction(jp, "get_todo", { id: 1 }, noCred);
    check("GET ok", got.outcome === "success" && got.status === 200);
    check("GET parsed body has title", typeof (got.data as any)?.title === "string", JSON.stringify(got.data).slice(0, 80));

    const created = await executeAction(jp, "create_post", { title: "hello", userId: 9 }, noCred);
    check("POST ok (201)", created.outcome === "success" && created.status === 201, String(created.status));
    check("POST echoed title", (created.data as any)?.title === "hello");
  } catch (e) {
    check("live HTTP reachable", false, String(e));
  }

  // --- D. pagination loop (fake fetch, deterministic) ---
  console.log("D. pagination (cursor, faked transport)");
  const pages = [
    { data: [1, 2], next: "c1" },
    { data: [3, 4], next: "c2" },
    { data: [], next: null },
  ];
  let calls = 0;
  const realFetch = globalThis.fetch;
  globalThis.fetch = (async () => {
    const body = pages[Math.min(calls, pages.length - 1)];
    calls++;
    return new Response(JSON.stringify(body), { status: 200, headers: { "content-type": "application/json" } });
  }) as typeof fetch;

  const pagMan = {
    manifestVersion: "1.0", key: "pg", name: "PG",
    connection: { baseUrl: "https://x.test" },
    auth: { type: "none" },
    paginators: { cur: { type: "cursor", itemsPath: "data", cursorRequestParam: "after", cursorResponsePath: "next", limitParam: "limit", pageSize: 2, maxPages: 10 } },
    actions: { list: { title: "List", request: { method: "GET", path: "/list" }, pagination: { $use: "cur" } } },
  } as unknown as ConnectorManifest;

  const listed = await executeAction(pagMan, "list", {}, noCred);
  globalThis.fetch = realFetch;
  check("pagination concatenates items", eq(listed.data, [1, 2, 3, 4]), JSON.stringify(listed.data));
  check("pagination stops on empty page", listed.pages === 3, String(listed.pages));
  check("pagination not truncated", listed.truncated === false);

  console.log(`\n${pass} passed, ${fail} failed`);
  process.exit(fail ? 1 : 0);
}

main();
