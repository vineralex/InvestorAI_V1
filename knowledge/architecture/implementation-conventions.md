---
type: policy
title: Solution Implementation Conventions
description: Approved dependency injection, object creation, and owner-only PostgreSQL verification rules across the Investor AI solution.
tags: [implementation, dependency-injection, conventions, verification, postgresql]
updated_at: 2026-10-09
status: approved
---

# Dependency injection

Application code does not create services or their dependencies with `new`.
The DI container creates them; dependencies are passed through constructors.
This rule applies across the entire solution, not only to `Program.cs` or the
Tenant database migrator.

DTOs, value objects, and technical objects such as
`NpgsqlConnectionStringBuilder`, streams, configuration builders, and DbUp
script descriptors are outside this restriction. DI containers and framework
builders may construct objects as part of their own composition mechanisms.

The rule does not require a Generic Host or a shared dependency-injection
framework. The Tenant one-shot migrator uses `ServiceCollection`; service
executables may use the DI container provided by their existing host.

Follow the [implementation discipline](../vertical-slices/portfolio-foundation/implementation-and-readiness/index.md)
when deciding whether an abstraction or another project is warranted.

# Shared infrastructure

Use one `InvestorAI.Infrastructure` project under BuildingBlocks for the current
shared configuration and database migration infrastructure. Organize by folders
and namespaces: `Configuration` contains DotEnvReader and its exception;
`DatabaseMigrations` contains migration configuration, settings, metadata, DI
registration, DbUp execution and safe logging. Migration configuration remains
with migrations because it maps service-owned migration credentials.

The four migrators reference this project. SQL resources, service definitions,
appsettings and launch profiles remain in their executable projects. Shared
infrastructure dependencies may be consumed transitively; separate projects
are introduced only when a concrete boundary requires them. This replaces the
earlier standalone `InvestorAI.DatabaseMigrations` project. Existing Messaging
and Observability projects are outside this reorganization.

# PostgreSQL verification ownership

The project owner performs all PostgreSQL verification. The assistant provides
step-by-step commands and queries, reviews the owner's reported results, and
records evidence in knowledge. The assistant must not independently execute
PostgreSQL checks, migrations, or database verification fixtures, including
those using isolated temporary containers.

Assistant verification is limited to source inspection and restore/build or
other checks that do not connect to PostgreSQL. Historical assistant database
verification evidence remains valid as historical evidence; this rule applies
to subsequent work. Record owner checks as complete only after actual reported
results, never merely because instructions were provided.

The owner does not use separate test databases for the current manual migrator
verification. The approved rollback check runs only through Tenant in the
owner's existing database, using temporary, distinctly named verification
tables and scripts. The owner executes the check and subsequent targeted
cleanup; existing application objects and migration records are preserved.
