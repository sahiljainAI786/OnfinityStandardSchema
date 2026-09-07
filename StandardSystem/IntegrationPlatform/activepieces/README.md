# activepieces/ — the rebranded fork

This directory is where our **forked, rebranded Activepieces CE** is wired in (as a git submodule).
It is NOT vendored into this repo; the fork lives in its own repository so it tracks upstream.

## Wiring it in

```bash
git submodule add <your-fork-url> activepieces
cd activepieces && git checkout brand-<pinned-tag>   # pinned release + our branding commits
```

- Branding lives only here; see [`../REBRAND.md`](../REBRAND.md) for the touchpoints and the
  upgrade-replay checklist.
- Build our branded image from this dir and push it as `BRAND_IMAGE` (referenced by the per-client
  stack template `../docker/docker-compose.client.yml`).
- Our connector runtime ships as a piece from [`../packages/universal-executor`](../packages/universal-executor),
  added to this fork's pieces — keep our code in new files so upstream merges stay clean.

## Rules

- Community edition only (`AP_EDITION=ce`). Never enable or copy `packages/ee` features.
- Keep the upstream `LICENSE` and copyright notice (MIT obligation).
