# Fork & rebrand Activepieces (MIT core) — playbook

Decision: rebrand the **MIT community edition** as a single brand (our platform), self-hosted on Linux.
Do **not** use `packages/ee` (commercial). The custom parts we add — connector-manifest schema,
Universal REST Executor, AI factory, Onfinity connector — are our own IP and unaffected by this.

> Paths below are by *area*, not verified filenames. Confirm exact files against the **pinned release**
> you fork (the repo is an Nx monorepo and moves fast). Treat this as the map, not the territory.

---

## 1. Legal guardrails (read first)

- **MIT lets us rebrand and sell.** We can change the name, logo, colors, and ship commercially.
- **MIT requires keeping the notice.** The original `LICENSE` file and copyright/permission notice
  must remain in the source. We rebrand the *product UI*; we do not delete the license or claim we
  authored the original engine.
- **EE is off-limits without a license.** Anything under `packages/ee` (and `packages/server/api/src/app/ee`)
  is commercial-licensed. Run **community edition only** and never enable/copy EE features to get branding.
- The convenient per-tenant white-label admin UI is an EE feature → we are doing single-brand source
  rebranding instead, which is the MIT-legal path.

## 2. Fork strategy that survives upgrades

The whole risk of a fork is drifting from upstream. Minimize it:

1. **Fork on GitHub**, clone, and add upstream:
   ```bash
   git remote add upstream https://github.com/activepieces/activepieces.git
   git fetch upstream --tags
   ```
2. **Pin to a release tag, never `main`.** Check out a specific released tag (e.g. the latest stable
   release) and base our `brand` branch on it. `main` is a moving target.
3. **Keep branding edits small, isolated, and documented.** Every change goes in this file's checklist
   (§4) so an upgrade is "re-apply N known edits," not "diff a mystery."
4. **Upgrade loop** (periodic):
   ```bash
   git fetch upstream --tags
   git checkout -b brand-<newtag> <newtag>
   git cherry-pick / merge our branding commits   # resolve conflicts using §4 as the checklist
   ```
   Prefer a handful of clean branding commits we can replay over a tangled merge.
5. **Edition + EE:** set `AP_EDITION=ce` and leave EE features disabled. Our `docker-compose` should
   never reference EE-only env flags.

## 3. Where branding lives (by area — confirm paths)

| Area | Where (confirm) | What to change |
|---|---|---|
| Logo & icon | `packages/react-ui` static assets (logo SVG/PNG, favicon) | Replace with our brand marks; match dimensions |
| Theme colors | `packages/react-ui` theme/Tailwind config or CSS variables | Set primary/accent to our palette |
| App / product name | frontend strings + page `<title>` + manifest/meta | Replace "Activepieces" in user-visible copy |
| Favicon & meta | `packages/react-ui` index/head + PWA manifest | Favicon, title, description, OG tags |
| Email templates | `packages/server` notification/email templates | Logo, product name, support/from address |
| External links | frontend footer/help links | Point to our docs/support, not activepieces.com |
| Login / first-run | frontend auth pages | Our logo + wording |

Note: some user-visible "Activepieces" strings are in i18n/copy and in component code — grep the
frontend for the name and review each hit (some are code identifiers you should NOT touch, only display copy).

## 4. Rebrand checklist (fill in as you go — this IS the upgrade replay list)

- [ ] Replace logo asset(s) — file: `__________`
- [ ] Replace favicon / app icons — file: `__________`
- [ ] Set theme primary/accent colors — file: `__________`
- [ ] Replace product name in display copy — files: `__________`
- [ ] Page `<title>` + meta/OG tags — file: `__________`
- [ ] PWA / web manifest name + icons — file: `__________`
- [ ] Email templates (logo, name, from-address) — files: `__________`
- [ ] Footer/help/docs links → our URLs — files: `__________`
- [ ] Login/first-run page branding — files: `__________`
- [ ] Confirm `LICENSE` + copyright notice retained (MIT compliance)
- [ ] Confirm `AP_EDITION=ce`, no EE features referenced
- [ ] Record exact file paths + commit hashes here for the next upgrade

## 5. Where our own code plugs in (separate from branding)

- **Universal REST Executor** → a custom piece under `packages/pieces` (or our own package), reading the
  connector manifest. This is our code, MIT-fork-safe.
- **AI connector factory** → standalone TS service in the monorepo (or a sibling app); emits manifests
  the executor consumes. Not part of Activepieces upstream → no merge conflicts.
- **Onfinity connector** → a manifest (`connectors/onfinity.connector.json`) loaded by the executor.

Keeping our code in *new* files/packages (not edits to upstream files) is what keeps upgrades cheap —
branding is the only place we touch upstream source, and §4 bounds that.

## 6. Open items

- Confirm with Activepieces sales the **exact EE-gated branding components** in the version we pin,
  so we don't rebrand on top of an EE file by accident.
- Decide repo shape: rebranded Activepieces fork as the umbrella monorepo, vs. Activepieces as a
  submodule with our executor/factory/connectors as sibling packages. (Recommend: fork as umbrella,
  our code in new packages — fewer moving parts.)
