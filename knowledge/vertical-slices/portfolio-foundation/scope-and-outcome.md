---
type: specification
title: Slice scope and outcome
description: Details for slice scope and outcome.
tags: [vertical-slice, portfolio, identity, tenancy, alpaca, architecture]
updated_at: 2026-10-09
status: draft
---

# Slice scope and outcome

## Status and authority

This bundle is the detailed architecture specification for the first vertical
slice identified in [MVP architecture](../../architecture/mvp-architecture/index.md).
It refines that approved architecture without changing the product boundary.
Until this bundle is reviewed and promoted, approved parent documents take
precedence over any accidental conflict.

Documents marked `status: intention`, including the future
[Laya and Jev Decision Layer cascade](../../architecture/decision-layer-laya-jev-cascade-intention.md),
are not inputs to the slice and are not approved dependencies.

## Outcome

The slice is complete when an operator can provision a tenant and initial
Owner, the Owner can activate and authenticate, connect and select one or more
Alpaca paper accounts, and see real imported positions in both account-level
and consolidated portfolio views. The Owner or Manager can request a manual
refresh and observe its progress and outcome. Every view discloses freshness,
coverage, degradation, and any safe user-facing errors.

Activation email or successful authentication alone does not demonstrate slice
readiness. The end-to-end user outcome is visible portfolio positions.

## Included scope

- protected Admin CLI provisioning of one tenant and its initial Owner;
- one-time email activation and password setup;
- authentication, access JWTs, refresh tokens, permissions, and tenant/resource
  authorization;
- asynchronous creation and validation of tenant-owned broker connections;
- explicit selection of accessible broker accounts;
- automatic asynchronous initial position import;
- manual tenant-level refresh across all included accounts;
- observation of connection, import, and refresh operations;
- latest successful account snapshots;
- account-level and consolidated portfolio reads;
- partial success, freshness, degradation, recovery, audit, and observability;
- Alpaca paper as the first broker adapter.

## Excluded scope

- Analysis Service, Decision Layer, Laya, Jev, LLM workflows, and analysis
  results;
- Scheduler Service and scheduled portfolio refresh;
- Web UI, self-service signup, and invitations;
- user-facing creation of Manager or Viewer memberships;
- automatic trading or any broker write action;
- live broker support, additional broker adapters, and real-time synchronization;
- cash, buying power, historical portfolio reconstruction, FX conversion, and
  base-currency totals;
- production or public-internet deployment;
- exact URLs, database schemas, classes, transport payloads, or cryptographic
  algorithms.


[Back to index](index.md)
