import { SeekPage } from '@activepieces/core-utils'
import { Template } from '@activepieces/shared'

// Curated Onfinity flow templates served in place of the Activepieces cloud catalog.
// Each is a Schedule-triggered flow (no external connection needed to import) whose action
// runs an Onfinity connector call through the "Onfinity Connectors" piece.

const PIECE = '@activepieces/piece-onfinity-connectors'
const SCHEDULE = '@activepieces/piece-schedule'

function onfinityAction(
  connector: string,
  action: string,
  input: Record<string, unknown>,
): unknown {
  return {
    name: 'step_1',
    type: 'PIECE',
    valid: true,
    settings: {
      input: { connector, action, input },
      pieceName: PIECE,
      actionName: 'run_connector_action',
      pieceVersion: '~0.0.1',
      sampleData: {},
      propertySettings: {
        connector: { type: 'MANUAL' },
        action: { type: 'MANUAL' },
        input: { type: 'MANUAL' },
      },
    },
  }
}

function scheduleFlow(displayName: string, nextAction: unknown): unknown {
  return {
    displayName,
    valid: true,
    notes: '',
    schemaVersion: '16',
    trigger: {
      name: 'trigger',
      type: 'PIECE_TRIGGER',
      valid: true,
      settings: {
        input: { timezone: 'UTC', hour_of_the_day: 8, run_on_weekends: true },
        pieceName: SCHEDULE,
        triggerName: 'every_day',
        pieceVersion: '~0.1.13',
        sampleData: {},
        propertySettings: {
          timezone: { type: 'MANUAL' },
          hour_of_the_day: { type: 'MANUAL' },
          run_on_weekends: { type: 'MANUAL' },
        },
      },
      nextAction,
    },
  }
}

function template(
  id: string,
  name: string,
  summary: string,
  description: string,
  flowName: string,
  action: unknown,
): unknown {
  return {
    id,
    created: '2026-07-01T00:00:00.000Z',
    updated: '2026-07-01T00:00:00.000Z',
    name,
    summary,
    description,
    type: 'OFFICIAL',
    platformId: null,
    status: 'PUBLISHED',
    flows: [scheduleFlow(flowName, action)],
    tables: [],
    tags: ['onfinity'],
    blogUrl: null,
    metadata: {},
    author: 'Onfinity',
    categories: ['Onfinity'],
    pieces: [SCHEDULE, PIECE],
  }
}

export const ONFINITY_TEMPLATES: Template[] = [
  template(
    'onfinity-insert-record',
    'Insert a record into Onfinity',
    'Create an Onfinity record on a schedule',
    'Runs on a daily schedule and inserts a record into Onfinity via VAAPI. Add your Onfinity credentials and adjust the table and SQL.',
    'Insert Onfinity record',
    onfinityAction('onfinity', 'insert_record', {
      tableName: 'C_BPartner',
      selectQuery:
        "INSERT INTO C_BPartner (AD_Client_ID, AD_Org_ID, Name, Value, IsActive) VALUES ({{connection.ad_client_id}}, {{connection.ad_org_id}}, 'Acme', 'ACME', 'Y', @pk)",
    }),
  ),
  template(
    'onfinity-get-record',
    'Fetch records from Onfinity',
    'Read Onfinity data on a schedule',
    'Runs on a daily schedule and reads records from Onfinity via VAAPI GetRecord. Use the returned rows in downstream steps.',
    'Fetch Onfinity records',
    onfinityAction('onfinity', 'get_record', {
      tableName: 'C_BPartner',
      selectQuery: 'SELECT Name, Value FROM C_BPartner',
    }),
  ),
  template(
    'onfinity-run-process',
    'Run an Onfinity process',
    'Trigger an Onfinity job/process on a schedule',
    'Runs on a daily schedule and executes an Onfinity process by its search key via VAAPI RunProcessBySearchKey.',
    'Run Onfinity process',
    onfinityAction('onfinity', 'run_process_by_search_key', {
      search_key: 'VA075_UpdateFSRStatus',
      record_id: 0,
      params: [],
    }),
  ),
] as unknown as Template[]

export function listOnfinityTemplates(search?: string): SeekPage<Template> {
  let items = ONFINITY_TEMPLATES
  if (search && search.trim().length > 0) {
    const q = search.toLowerCase()
    items = items.filter((t) =>
      `${t.name} ${(t as { summary?: string }).summary ?? ''} ${(t as { description?: string }).description ?? ''}`
        .toLowerCase()
        .includes(q),
    )
  }
  return { data: items, next: null, previous: null } as unknown as SeekPage<Template>
}
