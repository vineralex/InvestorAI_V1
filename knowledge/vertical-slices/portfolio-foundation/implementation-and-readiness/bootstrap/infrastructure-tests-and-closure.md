---
type: specification
title: Infrastructure, tests, and milestone closure
description: Recorded bootstrap implementation and verification evidence.
tags: [vertical-slice, implementation-order, testing, readiness, acceptance]
updated_at: 2026-10-09
status: draft
---

# Infrastructure, tests, and milestone closure


On 2026-10-09, the owner approved replacing the shared DatabaseMigrations
project with `InvestorAI.Infrastructure`, organized into Configuration and
DatabaseMigrations folders/namespaces. All four executable references were
updated. DotEnvReader is in Configuration; migration-specific settings and
configuration remain in DatabaseMigrations. Package versions, service SQL
resource names, journals and migration behavior are preserved. PostgreSQL
execution after this reorganization remains owner-only and is not claimed here.
Solution restore/build passed after the move: all 14 projects, zero errors,
and the same six existing analyzer warnings (Email Worker and Program classes).
The obsolete project folder and its bin/obj were removed, together with old
DatabaseMigrations DLL/PDB copies in the four migrator outputs. A full solution
rebuild passed during the 2026-10-09 audit. All solution entries resolve to
existing files. Reflection confirmed exactly the original 001 SQL resource in
each executable. All four executables rejected an intentionally unknown process
environment with exit code 1 and the expected safe startup message, exercising
DI startup without reading dotenv or connecting to PostgreSQL. Source review
confirmed the four definitions retain their own assembly, role, password key,
journal and resource prefix. No PostgreSQL verification was performed in this
audit; historical owner database evidence above predates the rename.

On 2026-10-09, the owner confirmed normal Tenant F5 execution after removal
of the temporary scripts and the Email journal row-count check as complete.

The owner authorized xUnit setup. `tests/InvestorAI.Infrastructure.Tests` is
included under the solution's tests folder and references the shared
infrastructure project. Central package versions are xunit.v3 4.0.1,
xunit.runner.visualstudio 4.0.0, and Microsoft.NET.Test.Sdk 18.10.1.
`global.json` selects Microsoft.Testing.Platform for .NET 10 CLI test execution;
the Visual Studio adapter supports Test Explorer. The test project is not packable.

The initial 11 test cases verify migration settings validation, valid port
boundaries, and literal password/service identity preservation in the connection
string. Configuration is synthetic and in-memory; tests neither read the root
`.env` nor connect to PostgreSQL. Solution build passed with the six existing
warnings and no errors; all 11 tests passed on 2026-10-09.

Run from the repository root:

```powershell
dotnet test --solution InvestorAI.slnx
```

In Visual Studio, open Test Explorer and run all tests. Service-specific test
projects will be added when they have concrete behavior to verify.

The [local development guide](../../../../../docs/local-development.md), linked from
the root [README](../../../../../README.md), collects the human-facing local setup
commands: configuration, restore/build/test, Compose startup and health status,
role bootstrap, all four migrators, journal inspection, and shutdown preserving
volumes. These commands were reviewed against repository configuration; this
documentation change does not claim a new owner-run environment verification.
On 2026-10-09, the owner confirmed that all four runtime roles reject CREATE
TABLE in the public schema using administrator-run SET LOCAL ROLE checks, with
ROLLBACK after each attempt. Together with the recorded build, test, bootstrap,
migrator, and infrastructure evidence, this closes milestone 1. Runtime DML
grants on future business objects remain subject to verification when those
objects are introduced; they are not established by this DDL check.

The owner performs the setup in Visual Studio; the assistant guides and reviews.
Discuss concrete changes before making them and obtain the owner's agreement;
do not automatically implement subsequent steps. The owner explicitly authorized
assistant implementation and isolated verification of this role bootstrap.
XML indentation uses tabs
with display width four; YAML indentation uses spaces.

Role bootstrap and runtime DDL denial are verified. Milestone 1 is complete.
Repeatable local setup commands are documented in the local development guide.


[Back to bootstrap index](index.md)
