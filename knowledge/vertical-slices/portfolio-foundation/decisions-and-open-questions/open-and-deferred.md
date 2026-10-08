---
type: decision-log
title: Open questions and deferred work
description: Details for open questions and deferred work.
tags: [vertical-slice, decisions, open-questions, portfolio]
updated_at: 2026-10-09
status: draft
---

# Open questions and deferred work

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
13. Full error-code catalogue, safe parameters, and fallback wording. Messages
    use English under the repository language rule in [AGENTS.md](../../../../AGENTS.md).
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
20. Mailpit and future container patch tags when their milestones are authored.
    Current Compose pins are PostgreSQL `18.6-bookworm`, RabbitMQ
    `4.3.6-management`, and pgAdmin `9.18`; these versions are already selected.
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
[Laya and Jev Decision Layer cascade intention](../../../architecture/decision-layer-laya-jev-cascade-intention.md)
is preserved for future research and is not a decision or dependency of this
slice.

[Back to index](index.md)
