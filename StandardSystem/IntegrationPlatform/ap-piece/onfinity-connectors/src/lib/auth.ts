import { PieceAuth } from '@activepieces/pieces-framework';

export const onfinityConnectorsAuth = PieceAuth.SecretText({
  displayName: 'Credentials (JSON)',
  description:
    'Credentials as JSON. Simple form: {"api_key":"..."} (treated as auth fields). Full form for connectors that also need connection details (e.g. Onfinity): {"fields":{"access_key":"...","secret_key":"..."},"connection":{"base_url":"https://your-onfinity","ad_client_id":1000005,"ad_org_id":1000008,"ad_user_id":1005527,"ad_role_id":1000118}}. Leave blank for connectors that need no auth.',
  required: false,
});
