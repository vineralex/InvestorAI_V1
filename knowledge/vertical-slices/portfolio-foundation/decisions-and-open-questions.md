---
type: decision-log
title: Portfolio Foundation Decisions and Open Questions
description: Accepted decisions, superseded alternatives, deferred capabilities, and unresolved design questions for the Portfolio Foundation slice.
tags: [vertical-slice, decisions, open-questions, portfolio]
updated_at: 2026-09-27
status: draft
---

# Decisions and open questions

## Accepted decisions

| ID | Decision | Rationale or consequence |
| --- | --- | --- |
| PF-001 | Tenant creation returns an observable asynchronous provisioning operation after local tenant/outbox commit. | Avoids pretending Identity and SMTP are part of one transaction. |
| PF-002 | Tenant lifecycle is `Provisioning → AwaitingOwnerActivation → Active`; provisioning failure is operation state. | The active-Owner invariant begins at `Active`. |
| PF-003 | Activation expiry and attempt limits are configurable; codes are hashed, single-use, and superseded on resend. | Security policy values are operational configuration rather than hardcoded constants. |
| PF-004 | Use short configurable access JWTs and rotated/revocable refresh tokens; issued access JWTs remain valid until expiry. | Preserves local JWT validation without a synchronous Identity lookup. |
| PF-005 | Permissions are independent authorization primitives; Owner, Manager, and Viewer are fixed permission bundles. | Services check capabilities, not role names; custom/direct grants are deferred. |
| PF-006 | Broker adapter is internal to Portfolio Service and owns provider translation only. | Avoids a Broker Connector Service and provider DTO leakage. |
| PF-007 | Connection and account are distinct; their cardinality is adapter-specific rather than a universal invariant. | Supports provider variation without modelling Alpaca behavior as universal truth. |
| PF-008 | Account discovery/access and portfolio inclusion are separate; Owner explicitly sets `included`. | Prevents every accessible account from silently entering the portfolio. |
| PF-009 | Connection validation is asynchronous with `PendingValidation`, `Ready`, `ActionRequired`, and `TemporarilyUnavailable`. | Broker latency/failure does not hold an HTTP request or create false success. |
| PF-010 | Inclusion automatically starts initial import. | No redundant manual refresh step is required. |
| PF-011 | Connection removal is a terminal disconnect: destroy credentials, exclude accounts, and create a new connection to resume. | Keeps the first slice simpler than a reversible reconnect lifecycle. |
| PF-012 | Every service owns stable error codes and a local user-message catalogue under a common envelope. | Enables friendly messages and future localization without a central dependency. |
| PF-013 | A tenant refresh captures its included-account target set at acceptance. | Results have deterministic coverage despite later selection changes. |
| PF-014 | At most one unfinished refresh exists per tenant; concurrent callers receive the same operation. | Prevents duplicate broker work and ambiguous status. |
| PF-015 | Portfolio PostgreSQL state coordinates refresh across instances. | Correctness cannot depend on process-local locks. |
| PF-016 | `Pending` is committed before queued execution; RabbitMQ delay does not make operation existence ambiguous. | API always returns a durable known operation. |
| PF-017 | Requests reuse an unfinished refresh and a terminal refresh within a configurable recent-result interval. | Treats rapid repeats as the accepted product policy without requiring client idempotency keys. |
| PF-018 | Refresh terminal states are `Succeeded`, `PartiallySucceeded`, and `Failed`; causes are a list of structured errors. | Multi-account work can report several account and operation errors. |
| PF-019 | One account has at most one in-flight broker fetch; initial import and tenant refresh reuse it. | Eliminates duplicate provider calls. |
| PF-020 | Transient account failures retry independently with configured backoff/limits; action-required failures do not. | One account does not block others and useless retries are avoided. |
| PF-021 | Account snapshots commit atomically and independently; there is no tenant-wide snapshot transaction. | Enables partial success while preventing partial account visibility. |
| PF-022 | Failed refresh retains the last successful account snapshot and marks it stale; no prior snapshot means unavailable. | Prevents data loss and misleading freshness. |
| PF-023 | Portfolio availability is `NotConfigured`, `Ready`, `Degraded`, or `Unavailable`. | Separates no setup, complete coverage, partial/stale coverage, and no usable coverage. |
| PF-024 | Both account-level and consolidated views are required. | Users need source detail and whole-portfolio interpretation. |
| PF-025 | Consolidation requires reliable instrument identity and matching currency, and preserves account breakdown. | Ticker text alone is not a safe merge key. |
| PF-026 | Currency is displayed per position; no cross-currency sums, per-currency totals, base currency, or FX exist in the slice. | Avoids incorrect financial aggregation before an FX design exists. |
| PF-027 | Broker credentials use envelope encryption; local dev uses a test `.env` key and deployed mode uses an external key manager. | Supports multiple Portfolio instances without storing the root key beside ciphertext. |
| PF-028 | Activation delivery uses permission-separated encryption: Identity encrypt-only, Email decrypt-only. | Keeps plaintext codes out of durable ordinary messages and limits key authority. |
| PF-029 | Each service stores local audit records in service-owned tables using a common contract-only library/format. | Preserves local transaction atomicity and service data ownership. |
| PF-030 | Local audit is shaped for a later asynchronous central projection, but no Audit Service exists in this slice. | Makes hybrid evolution easy without a new critical dependency. |
| PF-031 | Poison messages go to DLQ after configured retries; replay requires explicit operator action and operations cannot remain stuck forever. | Makes terminal failure and recovery observable. |
| PF-032 | The slice requires structured logs, OpenTelemetry-compatible traces, metrics, health checks, and configurable stuck/backlog alerts. | Observability is part of readiness rather than later polish. |
| PF-033 | Manager/Viewer permissions are implemented and tested with prepared memberships, but their onboarding/invitations are deferred. | Meets authorization design without expanding the first product flow. |
| PF-034 | Refresh with zero included accounts is rejected synchronously and portfolio remains `NotConfigured`. | Avoids meaningless long-running operations. |
| PF-035 | The first slice and MVP are local/dev only; public/production exposure is not a readiness goal. | Matches project intent and current security posture. |
| PF-036 | Public endpoint rate limiting is deferred from the first slice but remains mandatory for the complete MVP. | It is not needed for the controlled local slice boundary but must not be forgotten. |
| PF-037 | Functional slice readiness requires the user to see real portfolio positions, not merely complete activation. | Anchors completion in vertical user value. |

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

## Superseded or rejected alternatives

| Alternative | Resolution |
| --- | --- |
| Wait for pending Owner or SMTP before returning tenant-creation success | Rejected in favor of observable asynchronous provisioning. |
| Treat one connection as universally one or universally many accounts | Rejected; cardinality belongs to the adapter. |
| Make `paper/live` a universal account field based on an Alpaca assumption | Rejected as an unsupported broker-specific generalization. |
| Automatically include every accessible account | Rejected; Owner selection is explicit. |
| Model account inclusion as a multi-state lifecycle | Rejected; a boolean is sufficient. |
| Reversible connection disable/reconnect in the first slice | Deferred; disconnect is terminal and a later MVP slice may add richer behavior. |
| Central Error Dictionary Service | Rejected; services own catalogues under one envelope. |
| Require a client idempotency key for manual refresh | Rejected for this slice; server-side unfinished/recent-operation reuse is the policy. |
| Coordinate refresh with an in-memory flag | Rejected because multiple Portfolio instances must remain correct. |
| One transaction for all account snapshots | Rejected in favor of per-account atomicity and honest mixed-time disclosure. |
| Aggregate monetary values across currencies | Rejected until base-currency/FX semantics exist. |
| Keep the deployed broker root key in `.env`, a mounted static file, or PostgreSQL beside ciphertext | Rejected in favor of an external key manager and envelope encryption. |
| Shared audit table or shared audit repository | Rejected because it violates service ownership and transaction boundaries. |
| Central Audit Service as the only source of truth | Rejected; future central audit is a projection over local authoritative records. |
| Treat lack of first-slice rate limiting as public-production risk acceptance | Not applicable: the approved deployment boundary is local/dev only. |

## Open questions within the slice

These questions must be resolved during focused contract or implementation
design. They are not permission to change the decisions above.

1. Exact API resource shapes, URLs, request/response fields, pagination, and
   versioning.
2. Exact request-identity mechanism that lets a caller safely recover a lost
   create-tenant or create-connection response without creating a duplicate.
3. Exact message names, payload schemas, compatibility rules, and retention.
4. Exact database tables, columns, constraints, indexes, RLS policies, and
   migration ordering within each service's ownership.
5. Exact permission identifiers and whether Manager/Viewer may read connection
   metadata beyond portfolio coverage and refresh state.
6. Exact bootstrap and rotation mechanism for narrow internal service JWTs or
   equivalent workload credentials.
7. Exact KMS/Vault product, cryptographic algorithms, key hierarchy, backup,
   recovery, and rotation procedures.
8. Exact broker-specific account identity and duplicate-connection rules.
9. Exact canonical instrument identity used for safe consolidation when provider
   identifiers differ.
10. Exact quantity, market-value, cost-basis, and average-price aggregation rules
   for reliably matched same-currency positions.
11. Exact configuration values and allowed ranges for token/code expiry,
   attempts, retry/backoff, timeouts, concurrency, freshness, recent-refresh
   reuse, leases, and retention.
12. Snapshot and operation-history retention, pruning, and archival policies.
13. Full error-code catalogue, safe parameters, initial language, and fallback
    wording.
14. Real SMTP provider/configuration, templates, and operator handling after
    terminal delivery failure. Mailpit is selected only as the local/test SMTP
    sink for the activation milestone.
15. Ownership and project layout of versioned cross-service command and fact
    contract assemblies.
16. Telemetry backend, local dashboards, alert transport, and trace/metric
    retention.
17. Exact operator authentication and credential provisioning for Admin CLI.
18. Whether a Portfolio-completed fact is emitted in the first slice when no
    current consumer requires it, or introduced with the first consumer.
19. Final policy and library choice for mocks and hand-written test doubles.
20. Exact PostgreSQL, RabbitMQ, Mailpit, and other container patch tags selected
    when each relevant runbook milestone is authored.
21. CI provider workflow details, triggers, caching, and the point at which the
    meaningful test suite makes CI a required delivery gate.

## Deferred beyond this slice

- self-service signup, invitations, and user-facing Manager/Viewer creation;
- reversible connection disable/reconnect;
- additional brokers and live-account support;
- public/production deployment hardening;
- configurable custom roles and per-user permission overrides;
- full localization and administrative editing of message catalogues;
- centralized audit projection;
- scheduling and automated refresh;
- cash, buying power, FX, base-currency totals, and historical portfolio views;
- Analysis Service, Decision Layer, Laya/Jev execution, strategy, watchlist,
  opportunity search, and analysis email delivery.

The separate
[Laya and Jev Decision Layer cascade intention](../../architecture/decision-layer-laya-jev-cascade-intention.md)
is preserved for future research and is not a decision or dependency of this
slice.
