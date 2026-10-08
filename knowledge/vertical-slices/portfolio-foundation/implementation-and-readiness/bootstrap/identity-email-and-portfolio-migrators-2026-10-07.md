---
type: specification
title: Identity, Email, and Portfolio migrators (2026-10-07)
description: Recorded bootstrap implementation and verification evidence.
tags: [vertical-slice, implementation-order, testing, readiness, acceptance]
updated_at: 2026-10-09
status: draft
---

# Identity, Email, and Portfolio migrators (2026-10-07)


All three are implemented using the shared library and the Tenant pattern:
service-owned DI registration and MigrationDefinition, appsettings with
localhost/5432/investorai and no password, a Development F5 profile, and an
embedded `001_verify_migrator.sql` containing SELECT 1.

| Service | Migration username | Password environment key | Journal in public | Resource prefix |
| --- | --- | --- | --- | --- |
| Identity | investorai_identity_migration | IDENTITY_MIGRATION_PASSWORD | identity_schema_migrations | Identity.Migrations. |
| Email | investorai_email_migration | EMAIL_MIGRATION_PASSWORD | email_schema_migrations | Email.Migrations. |
| Portfolio | investorai_portfolio_migration | PORTFOLIO_MIGRATION_PASSWORD | portfolio_schema_migrations | Portfolio.Migrations. |

Each definition points to its own executable assembly. SQL stays service-owned.
Tenant's resource names and journal are unchanged. Restore and full solution
build passed with no errors; analyzer warnings remain for the existing Email
Worker logging and unsealed Program classes. The assistant did not execute
PostgreSQL checks for this change.

Owner verification on 2026-10-07 confirmed first and repeat F5 runs for Identity,
Email, and Portfolio, all with exit code 0. Each first run applied its own 001
script; repeat runs reported completion without applying it again. Identity's
journal retained one row and its timestamp; the owner confirmed journal
ownership and isolation. Email's supplied pgAdmin results confirmed owner
`investorai_email_migration` and access only for that role and `investorai_admin`;
the journal row-count check was not explicitly reported. Portfolio's owner
confirmed one row, corresponding migration-role ownership, and access only for
that role and the administrator. Tenant's post-extraction F5 run exited 0
without reapplying its existing migration.

Post-extraction rollback verification passed through owner-run Tenant checks
on 2026-10-07 in the existing database, with no separate test database. Temporary
scripts 002_manual_rollback_before, 003_manual_rollback_failure, and
004_manual_rollback_after create distinctly named verification tables; script
003 deliberately divides by zero after CREATE TABLE. The process reported
script 003 failure with SQLSTATE 22012 and exited 1. Owner pgAdmin results
confirmed that 002 remained committed and journaled, 003 rolled back both DDL
and journaling, and 004 never ran: only the before table existed, and the
journal contained only 001 and 002. The owner then successfully committed
targeted cleanup (DROP of the before table and DELETE of the exact 002 journal
record). The assistant removed all three temporary SQL files. No corrected-file
retry was performed in this manual check. Existing migration 001 was preserved.

The owner explicitly changed verification ownership: all PostgreSQL checks are
performed by the owner, including any temporary database fixtures. The assistant
provides steps, reviews reported results, and performs only non-database checks.
See [implementation conventions](../../../../architecture/implementation-conventions.md).
This supersedes earlier permission to run assistant-owned database fixtures for
subsequent work; historical results above remain historical evidence.


[Back to bootstrap index](index.md)
