---
type: decision-log
title: Accepted slice decisions
description: Details for accepted slice decisions.
tags: [vertical-slice, decisions, open-questions, portfolio]
updated_at: 2026-10-09
status: draft
---

# Accepted slice decisions

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


[Back to index](index.md)
