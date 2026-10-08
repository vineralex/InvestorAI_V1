---
type: specification
title: PostgreSQL role bootstrap (2026-10-06)
description: Recorded bootstrap implementation and verification evidence.
tags: [vertical-slice, implementation-order, testing, readiness, acceptance]
updated_at: 2026-10-09
status: draft
---

# PostgreSQL role bootstrap (2026-10-06)


The approved bootstrap is implemented in
[`infra/postgres/bootstrap.ps1`](../../../../../infra/postgres/bootstrap.ps1) and
[`001_bootstrap.sql`](../../../../../infra/postgres/sql/001_bootstrap.sql).
Visual Studio groups these files under `SolutionItems/Postgres` and its `SQL`
subfolder without moving their physical locations.

The bootstrap uses the existing database and the standard `public` schema.
Tenant, Identity, Email, and Portfolio each have two LOGIN roles named
`investorai_<service>_migration` and `investorai_<service>_runtime`. New roles
have no superuser, database/role creation, replication, or RLS-bypass authority.
All receive CONNECT and schema USAGE; only migration roles receive schema
CREATE. PUBLIC loses database CREATE/TEMPORARY and schema CREATE. Migration
roles own the objects they create. Service-owned DbUp migrations will grant
runtime access to specific objects; bootstrap grants no blanket table access.

Existing roles are skipped without checking or repairing their settings,
memberships, or passwords. Initial grants are reapplied. Restricted-role
guarantees therefore apply to newly created roles, not arbitrary pre-existing
roles. Role password rotation requires a separate deliberate change.

Each numbered `NNN_*.sql` file must tolerate repeated execution and contain
its own BEGIN/COMMIT. The runner executes files by name, stops at the first
error, and maintains no journal. A failed file rolls back; previously committed
files remain applied. Add future files to `SolutionItems/Postgres/SQL` explicitly.

#### Local run command

Requires PowerShell 7+, Docker Compose, and an already running PostgreSQL
service from this repository's Compose project. The configured administrator
must be a superuser (the initial POSTGRES_USER created by this Compose image).
The runner disables statement/error-statement logging for its administrator
session before handling role passwords and suppresses raw client diagnostics.

Keep the root `.env`; add missing keys from [`.env.example`](../../../../../.env.example).
Set POSTGRES_DB, POSTGRES_USER, POSTGRES_PASSWORD and separate
`<SERVICE>_MIGRATION_PASSWORD` / `<SERVICE>_RUNTIME_PASSWORD` values for TENANT,
IDENTITY, EMAIL, and PORTFOLIO. Values are literal single lines; optional outer
quotes are removed, with no evaluation, variable expansion, or escape processing.
Passwords travel through stdin, not command arguments. Compose interpolation
uses placeholders for container selection rather than interpreting real secrets.

From the repository root:

```powershell
docker compose up -d
.\infra\postgres\bootstrap.ps1
```

If using Windows PowerShell, invoke the script with `pwsh -NoProfile -File`.
There is no AdminCli database bootstrap command. This script does not start
containers, create the database, or configure DbUp.

#### Verification evidence

On 2026-10-06, an isolated PostgreSQL `18.6-bookworm` Compose fixture passed:

- eight new roles with the expected attributes, no role memberships, and correct
  database/schema permissions;
- repeated bootstrap with different supplied passwords preserves stored passwords;
- TCP authentication with a literal password containing quotes, backslash,
  dollar sign, backtick, hash, equals sign, spaces, and Unicode;
- migration table creation/removal, runtime permanent/temporary DDL denial,
  runtime DML after explicit grants, and denial for a neighboring service;
- existing role settings are deliberately left unchanged;
- per-file rollback, previous-file commit preservation, stop after failure,
  corrected-file recovery, and repeated execution of multiple numbered files;
- missing configuration rejection and no role secrets in runner output or
  PostgreSQL logs.

The temporary test Compose project was removed after verification. The isolated
checks did not change the owner's working database, `.env`, or working volumes.
This proves bootstrap behavior, not application RLS, service DbUp setup, or
complete milestone 1 readiness.

#### Owner environment verification (2026-10-06)

The owner populated the eight service-role passwords in the local `.env`,
started the full environment with `docker compose up -d`, and ran
`.\infra\postgres\bootstrap.ps1` against the working `investorai` database.
The output confirmed `Applied 001_bootstrap.sql.` and
`PostgreSQL bootstrap completed.` The owner subsequently confirmed that the
repeat-run check works.

The owner also inspected the roles in pgAdmin. The supplied query result shows
all eight service roles with LOGIN and schema USAGE; schema CREATE is true only
for migration roles. SUPERUSER, CREATEDB, CREATEROLE, BYPASSRLS, and database
TEMPORARY are false for all eight. This is working-environment evidence for
these attributes and privileges; actual DDL/DML denial and password preservation
were exercised separately in the isolated fixture above.

To repeat this read-only inspection in pgAdmin, select the `investorai` database
and open Query Tool as the configured administrator:

```sql
SELECT
    rolname,
    rolcanlogin,
    rolsuper,
    rolcreatedb,
    rolcreaterole,
    rolbypassrls,
    has_schema_privilege(rolname, 'public', 'USAGE') AS schema_usage,
    has_schema_privilege(rolname, 'public', 'CREATE') AS schema_create,
    has_database_privilege(
        rolname, current_database(), 'TEMPORARY'
    ) AS temporary_tables
FROM pg_roles
WHERE rolname IN (
    'investorai_tenant_migration', 'investorai_tenant_runtime',
    'investorai_identity_migration', 'investorai_identity_runtime',
    'investorai_email_migration', 'investorai_email_runtime',
    'investorai_portfolio_migration', 'investorai_portfolio_runtime'
)
ORDER BY rolname;
```


[Back to bootstrap index](index.md)
