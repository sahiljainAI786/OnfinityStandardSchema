# Onfinity Smart API — Linux / Docker deployment

The Smart API is plain ASP.NET Core 8 — fully cross-platform, stateless, no database.
On Linux it runs as a container (Kestrel inside), reachable on the LAN.

## Prerequisites (on the Linux server)
- Docker Engine + Compose plugin (`docker --version`, `docker compose version`)
- Network reach from this server to your VAAPI endpoint (erplive over the internet, or the
  Onfinity host's LAN address for an on-prem install)

## Deploy

1. Copy the `SmartAPI/` folder to the server (git, `scp -r`, or rsync). Only the build context is
   needed: `Dockerfile`, `.dockerignore`, `docker-compose.yml`, `src/`, `manifests/`.
   (The `.dockerignore` already excludes `.erplive.env`, `bin/`, `obj/`, `publish*/` — secrets and
   artifacts never enter the image.)

2. Set the VAAPI endpoint in `docker-compose.yml` (`Vaapi__BaseUrl`) — keep erplive, or point at the
   Onfinity host's LAN address for same-network. **No credentials go here** — callers send
   `accessKey`/`secretKey` per request (pass-through auth).

3. Build + run:
   ```bash
   cd SmartAPI
   docker compose up -d --build
   ```

## Verify

```bash
curl http://localhost:8090/health                       # {"status":"ok"}
curl http://localhost:8090/swagger/index.html -I        # 200
curl -H "accessKey: <k>" -H "secretKey: <s>" http://localhost:8090/v1/_entities
# dry-run resolution against VAAPI (read-only, no write):
curl -X PUT "http://localhost:8090/v1/BusinessPartner?dryRun=true" \
  -H "accessKey: <k>" -H "secretKey: <s>" -H "Content-Type: application/json" \
  -d '{"key":"VAI159","name":"Sunny","group":"Employees"}'
```

From other machines on the LAN: `http://<linux-host-ip>:8090`. Open the firewall:
`sudo ufw allow 8090/tcp` (or your firewall's equivalent).

## TLS (do this before real use — callers send keys)

Terminate TLS at nginx in front of the container:
```nginx
server {
    listen 443 ssl;
    server_name smartapi.your-lan.example;          # or the server IP with a self-signed/internal cert
    ssl_certificate     /etc/ssl/smartapi.crt;
    ssl_certificate_key /etc/ssl/smartapi.key;
    location / {
        proxy_pass         http://127.0.0.1:8090;
        proxy_set_header   Host $host;
        proxy_set_header   X-Forwarded-For $proxy_add_x_forwarded_for;
        proxy_set_header   X-Forwarded-Proto $scheme;
    }
}
```
For a LAN-only service without a public hostname, use an internal CA or a self-signed cert. With a
real hostname, use certbot/Let's Encrypt (same as the TimeTracker deployment).

## Operations

- **Logs:** `docker compose logs -f smartapi`
- **Update code:** redeploy the folder, then `docker compose up -d --build`
- **Update manifests only:** rebuild as above, OR uncomment the `volumes:` mount in
  `docker-compose.yml` to bind `./manifests` read-only and just `docker compose restart`.
- **Stateless:** nothing to back up — no DB, no volumes (unless you add the manifest mount).
- **Scale/move:** it's a stateless image; run it on any LAN host, or beside the IntegrationPlatform.
