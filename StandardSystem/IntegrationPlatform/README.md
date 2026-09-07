# Onfinity Integration Platform

An iPaaS that connects Onfinity (ViennaAdvantage / VAAPI) to hundreds of external apps via
AI-generated, **declarative connectors**. Built on a rebranded **Activepieces CE** fork (MIT),
deployed **one isolated stack per client** behind a shared **control plane**.

See the architecture decisions in the project memory; the load-bearing contract is the
[connector manifest schema](schema/connector-manifest.schema.json).

## Topology

```
SHARED (run once)                         PER CLIENT (one isolated stack each)
┌─────────────────────────────┐           ┌──────────────────────────────┐
│ control-plane               │  provisions │ app  (rebranded AP CE)       │
│  · client registry          │ ─────────▶ │ postgres (vault lives here)  │
│  · provisioner (compose/k8s)│           │ redis                        │
│  · catalog distribution     │           └──────────────────────────────┘
│ connector-factory (AI)      │           ┌──────────────────────────────┐
│  · OpenAPI → manifest       │ ─pushes──▶ │ ... client B ...             │
│ connector catalog (manifests)           └──────────────────────────────┘
└─────────────────────────────┘                       ... client N ...
```

Connector **definitions** are central and shared; connector **execution + credentials** are
isolated per client. One client's secrets, run logs, and load never touch another's.

## Layout

```
IntegrationPlatform/
├── schema/                     connector-manifest JSON Schema + design notes
├── connectors/                 the catalog — one *.connector.json per app (stripe, onfinity, …)
├── docker/
│   ├── docker-compose.client.yml        per-client stack TEMPLATE (app + postgres + redis)
│   ├── client.env.example               per-client parameters the control plane fills in
│   └── docker-compose.controlplane.yml  the shared control-plane + its registry DB
├── packages/
│   ├── shared/                 TS types mirroring the manifest schema (one source of truth)
│   ├── control-plane/          provisioning API + client registry + catalog distribution
│   ├── connector-factory/      AI: OpenAPI spec → connector manifest  (skeleton)
│   └── universal-executor/     the AP piece that runs any manifest    (skeleton)
├── activepieces/               the rebranded fork (git submodule — see REBRAND.md)
└── REBRAND.md                  fork-and-rebrand playbook
```

## Quickstart (dev, single host)

```bash
pnpm install
# 1. bring up the control plane + its registry db
docker compose -f docker/docker-compose.controlplane.yml up -d
# 2. provision a client stack via the control-plane API
curl -X POST localhost:8080/clients -d '{"id":"acme","name":"Acme Inc"}' -H 'content-type: application/json'
# the control plane stamps out docker-compose.client.yml as project ap-acme
```

> The control plane drives `docker compose` on a single host for dev. In production swap the
> `DockerComposeProvisioner` for a Kubernetes/Nomad provisioner — same interface.

## Status

Scaffold. `control-plane` is a working skeleton (JSON registry, compose provisioner, file catalog);
`connector-factory` and `universal-executor` are stubs with their contracts documented.
