---
type: specification
title: Shared migration infrastructure (2026-10-07)
description: Recorded bootstrap implementation and verification evidence.
tags: [vertical-slice, implementation-order, testing, readiness, acceptance]
updated_at: 2026-10-09
status: draft
---

# Shared migration infrastructure (2026-10-07)


The reusable configuration, dotenv reading, connection settings, DbUp execution,
safe output, and common DI registration are extracted into
the shared BuildingBlocks library, initially named `InvestorAI.DatabaseMigrations`
and renamed to `InvestorAI.Infrastructure` on 2026-10-09. All four migrators
reference the Infrastructure project.

`MigrationDefinition` carries service-owned metadata: service name, migration
username, password environment key, journal schema/table, SQL assembly, and
resource prefix. The shared code contains no Tenant-specific username, password
key, journal name, resource prefix, or SQL scripts. DI creates the common
services and supplies the definition as an immutable value object.

Tenant retains Program.cs, its AddTenantMigrator registration, appsettings,
launch profile, and embedded SQL. Its definition points at the Tenant executable
assembly, not the shared library. Resource names remain `Tenant.Migrations.*`,
the journal remains `public.tenant_schema_migrations`, and configuration priority,
per-script transactions, output, exit codes, and connection application name
are preserved. Existing journal entries therefore continue to match.

The shared project is justified by the approved additional migrator consumers.
It owns migration mechanics only, not business schemas or cross-service data.


[Back to bootstrap index](index.md)
