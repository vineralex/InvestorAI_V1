---
type: specification
title: 1. Repository bootstrap
description: Scope and exit gate for this implementation milestone.
tags: [vertical-slice, implementation-order, testing, readiness, acceptance]
updated_at: 2026-10-09
status: draft
---

# 1. Repository bootstrap


The project owner executes this milestone from a step-by-step runbook. It
creates a reproducible repository foundation without implementing business
behavior or pretending that unresolved contracts are settled.

Current solution shape:

```text
src/
  Tenant/
    InvestorAI.Tenant.Service
    InvestorAI.Tenant.DatabaseMigrator
  Identity/
    InvestorAI.Identity.Service
    InvestorAI.Identity.DatabaseMigrator
  Email/
    InvestorAI.Email.Worker
    InvestorAI.Email.DatabaseMigrator
  Portfolio/
    InvestorAI.Portfolio.Service
    InvestorAI.Portfolio.DatabaseMigrator
    InvestorAI.Portfolio.Providers.Alpaca
  BuildingBlocks/
    InvestorAI.Infrastructure
    InvestorAI.Observability
    InvestorAI.Messaging
tools/
  InvestorAI.AdminCli
  InvestorAI.DevCli
tests/
  InvestorAI.Infrastructure.Tests
```

Bootstrap work:

- pin .NET 10 through `global.json` and create `InvestorAI.slnx`;
- centralize common SDK project properties in `Directory.Build.props` and NuGet
  versions in `Directory.Packages.props`;
- enable nullable reference types and the agreed moderate analyzer policy;
- keep service code in the executable projects without separate Domain,
  Application, or per-service Infrastructure projects; shared configuration
  and migration infrastructure lives in the approved InvestorAI.Infrastructure
  building block;
- prepare one-shot service-owned migrators using Npgsql, DbUp, embedded
  SQL resources, separate journals, and distinct migration/runtime role
  configuration; Dapper is selected for future application persistence and
  is not required by the current migrators;
- provide Docker Compose services for PostgreSQL 18 and RabbitMQ 4.3, plus
  pgAdmin as the owner's local database client, using pinned patch tags and
  localhost-safe port exposure;
- establish xUnit as the test framework and reserve Testcontainers for tests
  that make PostgreSQL or RabbitMQ correctness claims;
- document repeatable restore, build, test, Compose start, health-check, and
  cleanup commands.

Bootstrap explicitly excludes business entities and workflows, database
schemas beyond the minimum needed to prove the migrator boundary, a universal
message bus, Mailpit, CI/CD, and speculative framework abstractions. Mailpit is
introduced with activation/email delivery, and CI/CD becomes a gate only after
a meaningful test suite exists.

Exit gate: the documented commands restore and build the full solution, run the
current test command successfully, start healthy PostgreSQL and RabbitMQ
containers, and demonstrate that runtime service credentials cannot perform
DDL. No source or runtime dependency points to `D:\Projects\InvestorAi`.


[Back to plan](index.md)
