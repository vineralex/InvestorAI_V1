---
type: concept
title: MVP Architecture
description: Service boundaries, analysis flows, delivery, and implementation slices for the Investor AI product MVP.
tags: [architecture, mvp, multitenancy, portfolio, analysis, scheduling, email]
updated_at: 2026-09-22
status: approved
---

# MVP architecture

This architecture implements the [MVP product definition](../product/mvp-product-definition.md).
It records system boundaries and execution flow, not service contracts. The
previously approved portfolio-retrieval slice remains the first implementation
milestone; the product MVP also requires two independent analyses, scheduled
execution, persistent results, and email delivery.

## Decisions

### Service and data ownership

| Component | Responsibility and owned data |
| --- | --- |
| Identity Service | Users, credentials, memberships, roles, activation, and token issuance. |
| Tenant Service | Tenant lifecycle. The Admin CLI remains its authenticated client. |
| Portfolio Service | Tenant broker connections and accounts, position snapshots, refresh operations, consolidated portfolio, and application-controlled market facts used for analysis. Alpaca paper remains the first broker adapter. |
| Analysis Service | Tenant Markdown strategy and maintained watchlist; both analysis operations, their run state and persisted results. It owns the Decision Layer as an internal boundary. |
| Scheduler Service | Tenant-scoped schedules and due-run initiation for each analysis operation. It does not own analysis results or make investment decisions. It may also initiate the existing portfolio refresh operation. |
| Email Service | Email composition and delivery state for activation and completed-analysis messages. It sends through SMTP; analysis results remain owned by Analysis Service. |

Services are independently deployable processes. RabbitMQ carries cross-service
requests and facts. PostgreSQL remains one database and schema with strict
service ownership: no service reads or writes another service's data directly.
Tenant-owned data is protected by application authorization and PostgreSQL
Row-Level Security, as specified in [tenant isolation](multi-tenant-data-isolation.md).
Docker Compose on one machine remains the initial deployment topology.

The approved infrastructure invariants remain in force: a user has one active
tenant membership in V1; broker connections belong to the tenant, which may
have multiple accounts contributing to one consolidated portfolio. Identity
Service uses .NET, ASP.NET Core Identity, and OpenIddict. The first broker
adapter is provider-neutral inside Portfolio Service; there is no Broker
Connector Service. Email Service uses SMTP with MailKit. Each service owns its
migrations and restricted runtime database role, with no shared repositories,
foreign keys, or cross-service database transactions. Portfolio Service keeps
broker credentials encrypted, with keys outside the database and repository;
credentials never enter messages or logs.

The Decision Layer evaluates bounded choices, classifications, scores, or
rankings for Analysis Service using the tenant strategy and controlled facts.
It does not own source data, authorize access, send email, or perform financial
actions. A deterministic implementation is sufficient initially. Explanations
may use an LLM, but generated prose cannot be the source of financial numbers;
the selected model and detailed Decision Layer contracts remain open.

### Common execution and delivery

An authorized API request or a due schedule initiates the same tenant-scoped
operation in Analysis Service. The service records a run, obtains strategy and
operation-specific inputs from their owners, evaluates them, and persists the
outcome before publishing an analysis-completed fact. API callers can observe
run status and retrieve persisted results. A scheduler run has the same result
and delivery path as an API run; neither operation invokes the other.

Email Service consumes each completed outcome and sends an email for that run,
including an explicit "nothing significant found" outcome. Delivery failures
are retried and remain visible as delivery state; they do not erase or recompute
the analysis result. Failed analysis runs are recorded and observable, but are
not completed outcomes. The exact recipient policy and operational handling of
repeated delivery failure remain open.

Cross-service messages retain tenant, operation, message, correlation, and
causation context. API boundaries validate user JWTs. Async commands use
narrowly scoped internal JWTs issued by Identity Service; user bearer tokens
never enter RabbitMQ. Delivery is at least once, so run initiation, result
publication, and email consumption must be idempotent and use outbox/inbox
patterns. These rules extend the [approved identity boundary](inbound-identity-and-authorization.md)
and existing portfolio-refresh choreography.

### Portfolio attention analysis

Analysis Service loads the tenant strategy and consolidated positions from
Portfolio Service, with current market facts and their freshness. It evaluates
positions through the Decision Layer, persists attention findings or an explicit
empty outcome, then publishes completion for email delivery. It may flag a
position for the user's attention; it never decides to sell or executes a trade.

Portfolio refresh remains a separate tenant-level operation. It covers all
active accounts, joins concurrent requests, and can partially succeed while
retaining the last successful snapshot for unavailable accounts and marking it
stale. The refresh API accepts work asynchronously and exposes an operation
identifier for completion tracking. An analysis uses only facts whose freshness
is known and makes incomplete coverage explicit in its result. If usable facts are
insufficient, it records a failed run rather than a misleading completed result.
Refresh and analysis cadence, and the threshold for sufficient facts, are open.

### Watchlist opportunity search

Analysis Service loads the tenant strategy and user-maintained watchlist, then
obtains controlled market facts for those candidates from Portfolio Service.
The Decision Layer ranks eligible candidates. Analysis Service persists several
ranked candidates or an explicit empty outcome and publishes completion for
email delivery. This operation does not require a preceding portfolio-attention
run, scan the whole market, compare positions with replacements, or choose an
asset to buy for the user. Missing or stale candidate facts are disclosed; the
minimum usable coverage remains open.

### Human control and access

Investor AI provides decision support only. It does not place, modify, or
cancel orders, change holdings, or make the user's final investment decision.
Owner and Manager may request refresh and analysis and read analysis results.
Viewer may read the portfolio but has no strategy or analysis actions. Owner
manages broker connections, tenant users, and tenant lifecycle. A tenant always
has an active Owner. Each service enforces tenant and resource authorization
independently.

Investor AI V1 is a new system, not a structural migration of the old one.
Existing domain rules, broker code, and tests require review before reuse; the
old database schema, Worker boundaries, and Quartz setup are not adopted.
The existing email implementation may be adapted to Email Service.

## Implementation slices

1. **Portfolio foundation.** Provision tenant and owner, activate with a
   one-time email code and password setup through the API, sign in, connect
   Alpaca paper accounts, import positions asynchronously,
   expose the consolidated portfolio, and support manual tenant refresh. This
   is the previously approved first slice, not the complete product MVP.
2. **Portfolio attention end to end.** Add Markdown strategy, controlled market
   facts, Analysis Service with its initial Decision Layer, persisted runs and
   results, API initiation and retrieval, scheduled initiation, and email for
   every completed attention analysis, including empty outcomes.
3. **Opportunities end to end.** Add tenant watchlist and candidate facts; use
   the same initiation, result, Decision Layer, scheduling, and email boundaries
   for independent watchlist ranking.
4. **MVP verification.** Demonstrate both API and scheduled paths end to end,
   tenant isolation, idempotent retries, data-freshness disclosure, result
   retrieval, and email delivery with observable failure states.

## Open questions

- Strategy interpretation, portfolio-risk signals, thresholds, opportunity
  criteria, and ranking semantics.
- Market-data provider and refresh/freshness policy, including minimum usable
  coverage for a completed analysis.
- Schedule cadence and tenant configuration for each operation and refresh.
- Detailed Decision Layer contracts, result shapes, and any future Jev or
  equivalent integration.
- Email recipients and escalation after persistent delivery failure.

These questions do not alter the approved product boundary. The MVP excludes
automatic trading, a Web UI, whole-market scanning, pairwise replacement
comparison, mandatory specialized ML, billing, realtime broker synchronization,
database-per-service, and Kubernetes. Agent Runtime, Agent Gateway, and Memory
Service remain [architectural intentions](intentions.md), not MVP dependencies.
