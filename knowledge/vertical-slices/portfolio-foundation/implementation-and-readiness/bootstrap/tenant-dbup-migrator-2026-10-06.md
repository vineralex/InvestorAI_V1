---
type: specification
title: Tenant DbUp migrator (2026-10-06)
description: Recorded bootstrap implementation and verification evidence.
tags: [vertical-slice, implementation-order, testing, readiness, acceptance]
updated_at: 2026-10-09
status: draft
---

# Tenant DbUp migrator (2026-10-06)


The Tenant migrator implements a one-shot console process with ServiceCollection
DI and no Generic Host. Its common infrastructure is now in
`src/BuildingBlocks/InvestorAI.Infrastructure`. Application services receive constructor dependencies;
the solution-wide rule is in
[implementation conventions](../../../../architecture/implementation-conventions.md).
Tenant was implemented first; the other three migrators now use the same
shared infrastructure. Owner database verification is recorded below.

Configuration priority is `appsettings.json`, then root `.env` in Development
only, then process environment. `Database__Host`, `Database__Port`,
`Database__Name`, and `Database__Password` override their corresponding process
aliases POSTGRES_HOST, POSTGRES_PORT, POSTGRES_DB, and TENANT_MIGRATION_PASSWORD.
Appsettings contains ordinary settings only: host `localhost`, port `5432`,
and database name `investorai`. These defaults can be overridden by dotenv or
process environment. No password is stored in JSON. Appsettings is copied to build/publish
output. `.env` and secrets are not copied. Username is fixed to
`investorai_tenant_migration`; administrator and runtime credentials are unused.

The F5 profile selects DOTNET_ENVIRONMENT=Development. The root is located by
walking upwards from AppContext.BaseDirectory to InvestorAI.slnx; the root
`.env` must exist. The parser uses the bootstrap's literal-value rules without
expansion or escape processing. Development defaults a blank host to localhost.
Production (also the default when the environment is absent) does not search
for `.env` or a repository; it also receives the JSON defaults. Container
configuration must override host when PostgreSQL runs in another container.
An explicitly blank Production host is rejected. Port defaults to 5432 and must be
1-65535. Database name and migration password are required. Unknown environment
names fail. Connection strings use NpgsqlConnectionStringBuilder.

Embedded SQL names are stable `Tenant.Migrations.<filename>`, ordered ordinally.
The first script, `001_verify_migrator.sql`, executes SELECT 1 without business
tables. DbUp records it in `public.tenant_schema_migrations`. Migration files
are RunOnce, must not be renamed/edited once applied, and have one DbUp-managed
transaction per file. Unlike infrastructure SQL, these files contain no explicit
BEGIN/COMMIT. No runtime or neighboring service receives journal privileges.

Success, including no pending migrations, exits 0. Failure exits 1 and stops
later scripts. Output is limited to script names, outcomes, safe validation
messages, and SQLSTATE where available; raw DbUp diagnostics are suppressed.
No database creation, bootstrap invocation, retries, or application startup is
performed. Run only one Tenant migrator instance at a time at this stage.

#### Owner run commands

In Visual Studio, select InvestorAI.Tenant.DatabaseMigrator as the startup
project and use its Development profile, then press F5. PostgreSQL and the role
bootstrap must already be ready. From the repository root, the equivalent is:

```powershell
dotnet run --project src/Tenant/InvestorAI.Tenant.DatabaseMigrator
```

Inspect the journal in pgAdmin, then repeat the run; the row count stays one:

```sql
SELECT * FROM public.tenant_schema_migrations;
```

Published execution without launch settings uses Production and environment
configuration. Container packaging and Compose wiring are a later step.
This implementation does not claim production deployment readiness.

#### Tenant verification evidence (2026-10-06)

Restore/build of the solution and Tenant publish passed. An isolated PostgreSQL
18.6 fixture, with a temporary localhost port and tmpfs storage, verified:

- DI resolution, Options validation, embedded resources, appsettings publication,
  and absence of dotenv in publish output;
- first-run journaling, repeat-run skipping, migration-role ownership, and no
  journal privileges for runtime/neighboring service roles;
- literal special-character/Unicode password authentication;
- JSON, dotenv, legacy process aliases, and standard Database__ variable
  precedence; Development lookup from an unrelated working directory;
- Production execution without a repository/dotenv, ignoring a malformed
  dotenv, and Production as the default environment;
- rejection of missing configuration, unknown environment, missing Development
  root/dotenv, malformed dotenv with a safe line number, and invalid ports;
- safe exit 1 for incorrect credentials and an unavailable database;
- a test-only failing embedded migration rolls back its DDL and journal entry,
  retains the earlier migration, and prevents the following script from running;
- correction/retry resumes the remaining scripts; an empty embedded-script
  assembly is rejected.

The failing and empty-script builds existed only in a temporary test fixture.
The dedicated test container was removed. The owner's `.env`, database and
working volumes were not changed. The fixture verifies the corresponding
Development path separately from the owner's Visual Studio verification below.
Milestone 1 remains incomplete.

#### Owner Tenant verification (2026-10-07)

The owner completed the step-by-step F5/pgAdmin checks in the working environment:

- First F5 run reported `Applied Tenant.Migrations.001_verify_migrator.sql.`,
  followed by `Tenant database migrations completed.`, and exited with code 0.
- pgAdmin showed exactly one journal row for the first embedded migration.
- The second F5 run reported completion without an Applied line and exited 0.
  The journal retained the same single row and applied timestamp.
- The journal owner is `investorai_tenant_migration`.
- All seven other service roles have no SELECT, INSERT, UPDATE, or DELETE
  privilege on `public.tenant_schema_migrations`.

These results confirm working-environment execution, journaling, repeat-run
skipping, ownership, and journal access isolation. Failure rollback remains
verified by the separate test fixture; no failing migration was applied to the
owner's database.


[Back to bootstrap index](index.md)
