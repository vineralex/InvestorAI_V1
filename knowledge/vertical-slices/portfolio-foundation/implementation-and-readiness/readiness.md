---
type: specification
title: Slice readiness criteria
description: Architecture, functional, and operational completion gates.
tags: [vertical-slice, implementation-order, testing, readiness, acceptance]
updated_at: 2026-10-09
status: draft
---

# Slice readiness criteria

## Architecture readiness criteria

- Every service-owned datum and state transition has one authoritative owner.
- Cross-service references and dependencies obey the ownership rules.
- Every synchronous response and asynchronous message has defined semantics.
- Lifecycle diagrams cover success, partial success, retries, terminal failure,
  and recovery.
- Tenant and account concurrency invariants hold across multiple instances.
- Transaction boundaries, outbox/inbox behavior, correlation, and DLQ handling
  are specified and verified.
- Credential and activation-secret protection has no unresolved plaintext path.
- Permissions are independent from roles and enforced by every API.
- Local audit and observability cover every long-running operation.
- Approved decisions and unresolved questions remain separate.
- No intention document has been treated as an approved dependency.

## Functional readiness criteria

- The complete end-to-end verification passes with a real Alpaca paper account.
- Both required portfolio views show correct current positions.
- Manual refresh is accepted asynchronously and observable to completion.
- Concurrent/repeated refresh requests follow the accepted reuse policy.
- Partial failure retains successful and prior usable account snapshots.
- `NotConfigured`, `Ready`, `Degraded`, and `Unavailable` are demonstrated.
- Empty successful accounts and unavailable never-imported accounts are
  distinguishable.
- Currency is shown per position and never misleadingly aggregated.
- Safe error lists can be resolved through each service's message catalogue.

## Security and operational readiness criteria

- The environment is explicitly local/dev and not publicly exposed.
- RLS and cross-tenant tests pass.
- The dev key provider is visibly non-production and deployed mode cannot use it.
- No credential, token, activation code, or private key appears in telemetry or
  persistent plaintext.
- Audit records exist for required security and business actions.
- Required metrics, traces, health checks, backlog/stuck-work signals, and DLQ
  behavior are demonstrable.
- Configuration values have validation and documented safe defaults.

## Not a completion claim for the product MVP

Passing this slice proves the Portfolio Foundation only. It does not prove the
MVP product promise, which additionally requires strategy-aware portfolio
attention analysis, opportunity search, scheduling, persisted results, and
completed-analysis email delivery.

[Back to implementation index](../index.md)
