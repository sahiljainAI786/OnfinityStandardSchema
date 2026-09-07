import { createAction, Property } from '@activepieces/pieces-framework';
import { httpClient, HttpMethod } from '@activepieces/pieces-common';
import { onfinityConnectorsAuth } from '../auth';

type CatalogEntry = { key: string; name?: string; actions?: string[] };

type Credential = {
  fields: Record<string, unknown>;
  connection?: Record<string, unknown>;
  tokens?: Record<string, unknown>;
};

// Accepts either the simple form {"api_key":"..."} (treated as auth fields) or the full form
// {"fields":{...},"connection":{...},"tokens":{...}} for connectors (e.g. Onfinity) that also need
// connection details like base_url / client & org IDs.
function parseCredential(raw: string | undefined): Credential {
  if (!raw) return { fields: {} };
  let parsed: unknown;
  try {
    parsed = JSON.parse(raw);
  } catch {
    return { fields: {} };
  }
  if (parsed && typeof parsed === 'object') {
    const obj = parsed as Record<string, unknown>;
    if ('fields' in obj || 'connection' in obj || 'tokens' in obj) {
      return {
        fields: (obj['fields'] as Record<string, unknown>) ?? {},
        connection: obj['connection'] as Record<string, unknown> | undefined,
        tokens: obj['tokens'] as Record<string, unknown> | undefined,
      };
    }
    return { fields: obj };
  }
  return { fields: {} };
}

function executorUrl(): string {
  // read the env without depending on @types/node (piece builds in a minimal image)
  const env = (globalThis as { process?: { env?: Record<string, string | undefined> } }).process?.env;
  return env?.['ONFINITY_EXECUTOR_URL'] ?? 'http://executor:3070';
}

async function getCatalog(): Promise<CatalogEntry[]> {
  const res = await httpClient.sendRequest<CatalogEntry[]>({
    method: HttpMethod.GET,
    url: `${executorUrl()}/catalog`,
  });
  return res.body ?? [];
}

export const runConnectorAction = createAction({
  auth: onfinityConnectorsAuth,
  name: 'run_connector_action',
  displayName: 'Run Connector Action',
  description:
    'Run an action on any connector in the Onfinity catalog (Stripe, Onfinity ERP, and more) via the Onfinity executor.',
  props: {
    connector: Property.Dropdown({
      displayName: 'Connector',
      description: 'Which external app / connector to use.',
      required: true,
      refreshers: [],
      options: async () => {
        const catalog = await getCatalog();
        return {
          disabled: false,
          options: catalog.map((c) => ({ label: c.name ?? c.key, value: c.key })),
        };
      },
    }),
    action: Property.Dropdown({
      displayName: 'Action',
      description: 'Which action to run on the selected connector.',
      required: true,
      refreshers: ['connector'],
      options: async (ctx) => {
        const connector = (ctx as Record<string, unknown>)['connector'] as string | undefined;
        if (!connector) {
          return { disabled: true, placeholder: 'Select a connector first', options: [] };
        }
        const catalog = await getCatalog();
        const entry = catalog.find((c) => c.key === connector);
        return {
          disabled: false,
          options: (entry?.actions ?? []).map((a) => ({ label: a, value: a })),
        };
      },
    }),
    input: Property.Object({
      displayName: 'Input',
      description: 'Action input fields (key → value), matching the action\'s schema.',
      required: false,
    }),
  },
  async run(context) {
    const connector = context.propsValue.connector as string;
    const action = context.propsValue.action as string;
    const input = (context.propsValue.input ?? {}) as Record<string, unknown>;

    const credential = parseCredential(context.auth as string | undefined);

    const res = await httpClient.sendRequest({
      method: HttpMethod.POST,
      url: `${executorUrl()}/run`,
      headers: { 'Content-Type': 'application/json' },
      body: { connector, action, input, credential },
    });
    return res.body;
  },
});
