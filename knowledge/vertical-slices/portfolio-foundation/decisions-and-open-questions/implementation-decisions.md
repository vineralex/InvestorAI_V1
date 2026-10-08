---
type: decision-log
title: Accepted implementation decisions
description: Details for accepted implementation decisions.
tags: [vertical-slice, decisions, open-questions, portfolio]
updated_at: 2026-10-09
status: draft
---

# Accepted implementation decisions

## Accepted implementation bootstrap decisions

These decisions define the executable repository bootstrap only. They do not
close the remaining API, message, database, or security contract questions and
do not constitute a complete implementation plan for the slice.

| ID | Decision | Rationale or consequence |
| --- | --- | --- |
| PF-038 | Delivery proceeds through independently verifiable milestones with mandatory exit gates. The first milestone is a step-by-step bootstrap runbook executed by the project owner. | Keeps progress reviewable and makes the initial setup educational and reproducible. |
| PF-039 | Use earned abstractions: add a project or abstraction only for a second concrete implementation, required dependency isolation, or an already approved invariant. | Prevents architecture-astronautics while preserving deliberate service boundaries. |
| PF-040 | Start each service with one executable project plus its database migrator rather than separate Domain, Application, and Infrastructure projects. Use folders and namespaces until a compile-time boundary earns a separate project. | Avoids empty layering projects and permits later extraction without changing service ownership. |
| PF-041 | The initial project set is `Tenant.Service`, `Identity.Service`, `Portfolio.Service`, `Email.Worker`, one `DatabaseMigrator` per service, `Portfolio.Providers.Alpaca`, `AdminCli`, `DevCli`, and minimal `Observability` and `Messaging` building blocks. | Creates only slice-owned deployables, explicit migration executables, the first provider boundary, and narrowly scoped shared technical code. |
| PF-042 | Standardize on .NET 10, `InvestorAI.slnx`, a pinned SDK in `global.json`, common MSBuild properties in `Directory.Build.props`, and NuGet Central Package Management in `Directory.Packages.props`. | Gives the repository one reproducible toolchain and avoids duplicated framework and package versions. |
| PF-043 | Enable nullable reference types and the recommended built-in .NET analyzers. Analyzer findings remain warnings by default; only selected correctness and security rules fail the build, and style rules do not block it. | Establishes useful static checks without turning formatting preferences or the full analyzer catalogue into delivery blockers. |
| PF-044 | Use Dapper and Npgsql for persistence and DbUp for explicit PostgreSQL migrations. Each service has embedded SQL resources, its own migration journal, a one-shot migrator using a migration role, and a restricted runtime role without DDL privileges. | Fits RLS, grants, partial indexes, outbox/inbox, and worker-claim SQL while keeping migration authority out of runtime services. |
| PF-045 | Bootstrap targets PostgreSQL 18 and RabbitMQ 4.3. Exact supported patch tags are pinned when the runbook is authored and updated deliberately; floating `latest` tags are prohibited. | Uses current stable major lines while keeping local environments reproducible. |
| PF-046 | Use xUnit as the test framework and Testcontainers for integration tests that claim PostgreSQL or RabbitMQ behavior. Docker Compose is for the complete local environment and end-to-end journeys. | Retains a familiar test framework and verifies infrastructure guarantees against real dependencies. |
| PF-047 | Distinguish targeted integration commands from published facts/events. Do not introduce MediatR, an internal CQRS bus, or a universal enterprise bus by default; shared messaging code grows only from concrete cross-service paths. | Preserves message semantics without creating an unused internal framework. |
| PF-048 | Keep provider implementations behind Portfolio-owned neutral capabilities; Alpaca is isolated in `Portfolio.Providers.Alpaca`, and provider SDK types do not leave that project. | Makes the first provider replaceable without splitting every infrastructure concern into a project prematurely. |
| PF-049 | `DevCli` is an intentionally privileged, developer-only diagnostic tool that may call application and adapter code directly. It is excluded from runtime deployment and cannot serve as unit, integration, security, or end-to-end evidence. | Supports personal exploration without weakening runtime boundaries or readiness claims. |
| PF-050 | Add Mailpit only with the activation/email milestone, not during repository bootstrap. Defer CI/CD until a meaningful test suite exists. | Introduces supporting infrastructure when a real behavior needs it rather than front-loading empty automation. |
| PF-051 | Bootstrap the existing PostgreSQL database and `public` schema using `infra/postgres/bootstrap.ps1` and numbered SQL files. Use two LOGIN roles per service: migration owns its objects; runtime receives specific object grants from service migrations. New runtime roles have no DDL, TEMPORARY, ownership, or BYPASSRLS authority. | Preserves service boundaries without an extra owner role or an AdminCli infrastructure feature. Business logic remains separate from PostgreSQL-specific SQL; multiple database providers are not a current goal. |
| PF-052 | Infrastructure SQL files are repeatable, each with its own transaction; execute all by number without a journal and stop on the first failure. Existing roles and passwords are neither checked nor repaired; initial grants are reapplied. Read literal passwords from root `.env` and pass them through stdin to containerized psql. Group scripts under `SolutionItems/Postgres/SQL`. | Keeps the local bootstrap simple. Previously committed files survive a later failure; existing-role restrictions are not guaranteed or audited. See [implementation and readiness](../implementation-and-readiness/index.md) for the run command and verification evidence. |

## Tenant migrator implementation decisions (2026-10-06)

| ID | Decision | Rationale or consequence |
| --- | --- | --- |
| PF-053 | Across the solution, application code does not construct services or their dependencies with `new`; DI creates them and supplies constructor dependencies. DTOs, value objects, and technical objects are exempt. | See [implementation conventions](../../../architecture/implementation-conventions.md). This does not require a Generic Host. |
| PF-054 | Implement and verify Tenant.DatabaseMigrator before the other three migrators. Use ServiceCollection, Options validation, and layered JSON/Development dotenv/process-environment configuration. Production never reads dotenv; migration username is fixed. | Supports F5 and published/container execution without duplicating local secrets or embedding them into deployment artifacts. |
| PF-055 | Tenant migrations are embedded RunOnce SQL resources with stable names, ordinal ordering, journal `public.tenant_schema_migrations`, and a DbUp-managed transaction per file. Start with SELECT 1. Exit 0 on success/no changes and 1 on failure; suppress raw diagnostics and stop later scripts. | Proves execution/journaling without business tables. Unlike bootstrap, DbUp journals completed scripts and owns transaction boundaries. See [implementation and readiness](../implementation-and-readiness/index.md). |

## Shared migrator extraction (2026-10-07)

| ID | Decision | Rationale or consequence |
| --- | --- | --- |
| PF-056 | Extract common migration infrastructure into BuildingBlocks; each executable supplies its own MigrationDefinition and embedded SQL. All four migrators now consume the shared code. The original `InvestorAI.DatabaseMigrations` project placement is superseded by PF-057. | Preserve resource names, journals, transactions, configuration priority, output, and exit codes so previously applied migrations remain recognized. |
| PF-057 | Use one `InvestorAI.Infrastructure` project for current shared configuration and migration dependencies, organized into Configuration and DatabaseMigrations folders/namespaces. DotEnvReader belongs to Configuration; migration-specific configuration stays with migrations. | Avoid premature project fragmentation. Service SQL, metadata registrations, appsettings and F5 profiles remain service-owned. Existing Messaging and Observability projects are outside this change. |


[Back to index](index.md)
