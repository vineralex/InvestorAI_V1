# Local development

[Back to README](../README.md)

## Prerequisites

- .NET SDK **10.0.401**, as pinned in `global.json`.
- Docker with Docker Compose v2 and Linux container support.
- PowerShell **7+** (`pwsh`) for the PostgreSQL bootstrap script.
- Visual Studio with .NET 10 support if using the IDE workflow.

Run the commands below from the repository root in PowerShell. Stop and resolve
any failure before continuing to the next step.

## Local configuration

Create `.env` only if it does not already exist:

```powershell
if (-not (Test-Path -LiteralPath .env)) {
    Copy-Item -LiteralPath .env.example -Destination .env
}
```

Fill all empty values using [`.env.example`](../.env.example) as the checklist.
For an existing `.env`, add missing keys without overwriting current values.

- `POSTGRES_DB`, `POSTGRES_USER`, and `POSTGRES_PASSWORD` initialize PostgreSQL.
- Each service has separate migration and runtime passwords.
- `RABBITMQ_USER`, `RABBITMQ_PASSWORD`, and `RABBITMQ_VHOST` initialize RabbitMQ.
- `PGADMIN_EMAIL` and `PGADMIN_PASSWORD` configure the local pgAdmin login.
- Keep `POSTGRES_HOST=localhost` and `POSTGRES_PORT=5432` for host-run migrators.

Do not commit `.env` or real credentials. Bootstrap and Development migrators
read literal single-line values with optional matching outer quotes. Docker
Compose has its own interpolation rules; single-quote values containing literal
`$` characters in `.env`.

Changing initialization credentials in `.env` does not rotate credentials in
existing database or RabbitMQ volumes. Existing service roles are also skipped
by bootstrap; their passwords are not changed on a repeat run.

## Restore, build, and test

```powershell
dotnet --version
dotnet restore InvestorAI.slnx
dotnet build InvestorAI.slnx --no-restore
dotnet test --solution InvestorAI.slnx --no-restore
```

Expected: SDK `10.0.401`, successful restore and build, and 11 passing test
cases. Six existing analyzer warnings remain in the Email Worker and migrator
Program classes. The tests use synthetic configuration and do not require
containers or connect to PostgreSQL.

In Visual Studio, open `InvestorAI.slnx`, build the solution, and run the tests
through Test Explorer.

## Start local infrastructure

Start Docker first, then run:

```powershell
docker compose up -d --wait
docker compose ps
```

Expected: `postgres` and `rabbitmq` are healthy; `pgadmin` is running. Published
ports bind to localhost:

| Service | Host endpoint |
| --- | --- |
| PostgreSQL | `localhost:5432` |
| RabbitMQ AMQP | `localhost:5672` |
| RabbitMQ management | <http://localhost:15672> |
| pgAdmin | <http://localhost:5050> |

PostgreSQL and RabbitMQ health checks confirm infrastructure availability, not
application readiness. This configuration is for local development.

## Bootstrap database roles

With PostgreSQL healthy and `.env` populated, run:

```powershell
pwsh -NoProfile -File ./infra/postgres/bootstrap.ps1
```

Expected: `Applied 001_bootstrap.sql.` followed by
`PostgreSQL bootstrap completed.` and exit code 0.

The script uses the configured PostgreSQL administrator to create migration
and runtime roles for Tenant, Identity, Email, and Portfolio. It does not create
the database or run service migrations. Repeating it preserves existing role
passwords and reapplies the initial grants.

## Run service migrations

Run each command separately and check its result before continuing:

```powershell
dotnet run --project src/Tenant/InvestorAI.Tenant.DatabaseMigrator --launch-profile InvestorAI.Tenant.DatabaseMigrator
dotnet run --project src/Identity/InvestorAI.Identity.DatabaseMigrator --launch-profile InvestorAI.Identity.DatabaseMigrator
dotnet run --project src/Email/InvestorAI.Email.DatabaseMigrator --launch-profile InvestorAI.Email.DatabaseMigrator
dotnet run --project src/Portfolio/InvestorAI.Portfolio.DatabaseMigrator --launch-profile InvestorAI.Portfolio.DatabaseMigrator
```

These profiles select Development and load the root `.env`. Process environment
variables override file settings. When using --no-launch-profile with no DOTNET_ENVIRONMENT set, the default is
Production, which does not load `.env`.

Expected for each service: `<Service> database migrations completed.` and exit
code 0. A first run also reports `Applied <Service>.Migrations.001_verify_migrator.sql.`
Repeat runs skip the applied script. Run only one migrator per service at a time.
The current SQL scripts execute `SELECT 1`; they do not create business tables.

In Visual Studio, select each DatabaseMigrator as the startup project, use its
Development profile, and press F5.

### Inspect migration journals

In pgAdmin, register PostgreSQL using host `postgres`, port `5432`, and the
database administrator credentials from `.env`. The host is `postgres` because
pgAdmin runs inside the Compose network.

Open Query Tool for the configured database and run:

```sql
SELECT 'Tenant' AS service, count(*) AS migration_count FROM public.tenant_schema_migrations
UNION ALL
SELECT 'Identity', count(*) FROM public.identity_schema_migrations
UNION ALL
SELECT 'Email', count(*) FROM public.email_schema_migrations
UNION ALL
SELECT 'Portfolio', count(*) FROM public.portfolio_schema_migrations;
```

Expected with the current migrations: one row per service, each with
`migration_count = 1`, including after repeat runs.

## Stop local infrastructure

```powershell
docker compose down
```

This removes the containers and Compose network while retaining the named
PostgreSQL, RabbitMQ, and pgAdmin volumes. Start again with
`docker compose up -d --wait`. Do not add `--volumes` unless you intend to delete
the stored local data.

