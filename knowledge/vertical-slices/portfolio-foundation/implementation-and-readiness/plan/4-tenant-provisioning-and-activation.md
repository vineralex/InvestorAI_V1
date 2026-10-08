---
type: specification
title: 4. Tenant provisioning and activation
description: Scope and exit gate for this implementation milestone.
tags: [vertical-slice, implementation-order, testing, readiness, acceptance]
updated_at: 2026-10-09
status: draft
---

# 4. Tenant provisioning and activation


- implement protected Admin CLI to Tenant Service acceptance;
- persist tenant/provisioning/audit/outbox atomically;
- establish pending Owner idempotently through Identity;
- implement activation challenge lifecycle, password setup, and Owner-activated
  fact;
- project all onboarding milestones in Tenant Service.

Exit gate: duplicate/out-of-order messages cannot create duplicate tenants,
memberships, Owners, or active codes, and a failed remote step is observable and
recoverable.


[Back to plan](index.md)
