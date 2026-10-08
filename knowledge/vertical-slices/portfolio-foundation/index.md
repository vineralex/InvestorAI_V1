---
type: specification
title: Portfolio Foundation Vertical Slice
description: Entry point for the Portfolio Foundation specification.
tags: [vertical-slice, portfolio, identity, tenancy, alpaca, architecture]
updated_at: 2026-10-09
status: draft
---

# Portfolio Foundation vertical slice


Read one relevant branch. Milestone 1 (repository bootstrap) is complete;
milestone 2 (contracts and isolation) is next. The full slice is not complete.

- [Scope and outcome](scope-and-outcome.md)
- [System context](system-context.md)
- [Invariants and dependencies](invariants-and-dependencies.md)

## Specification map

| Area | Document |
| --- | --- |
| Service duties, data ownership, and allowed references | [Service ownership](service-ownership.md) |
| Provisioning, activation, JWTs, permissions, and roles | [Onboarding and access](onboarding-and-access/index.md) |
| Broker adapter, connection/account lifecycle, and credentials | [Broker connections](broker-connections/index.md) |
| Imports, refresh, snapshots, freshness, and portfolio views | [Portfolio refresh and views](portfolio-refresh-and-views/index.md) |
| Conceptual operations/messages, sequences, transactions, and recovery | [Cross-service contracts](cross-service-contracts/index.md) |
| Security, local audit, logs, metrics, traces, and health | [Security, audit, and observability](security-audit-observability/index.md) |
| Delivery order, verification, and readiness gates | [Implementation and readiness](implementation-and-readiness/index.md) |
| Accepted decisions, deferred work, and unresolved questions | [Decisions and open questions](decisions-and-open-questions/index.md) |

[Back to parent index](../index.md)
