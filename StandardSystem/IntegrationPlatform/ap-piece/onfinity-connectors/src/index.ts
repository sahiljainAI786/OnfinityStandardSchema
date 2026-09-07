import { createPiece, PieceCategory } from '@activepieces/pieces-framework';
import { onfinityConnectorsAuth } from './lib/auth';
import { runConnectorAction } from './lib/actions/run-connector-action';

export { onfinityConnectorsAuth };

export const onfinityConnectors = createPiece({
  displayName: 'Onfinity Connectors',
  description:
    'Run any Onfinity-catalog connector action (Stripe, Onfinity ERP, and more) through the Onfinity executor — one step for every connector.',
  auth: onfinityConnectorsAuth,
  minimumSupportedRelease: '0.36.1',
  logoUrl: '/branding/onfinity-mark.png',
  categories: [PieceCategory.CORE],
  authors: ['onfinity'],
  actions: [runConnectorAction],
  triggers: [],
});
