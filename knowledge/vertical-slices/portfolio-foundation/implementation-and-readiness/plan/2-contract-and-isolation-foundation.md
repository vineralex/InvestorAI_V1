---
type: specification
title: 2. Contract and isolation foundation
description: Scope and exit gate for this implementation milestone.
tags: [vertical-slice, implementation-order, testing, readiness, acceptance]
updated_at: 2026-10-09
status: draft
---

# 2. Contract and isolation foundation


- establish service projects/process boundaries and independent database roles;
- establish tenant context propagation and RLS test harness;
- define common message, error, and audit envelopes without shared persistence;
- establish transactional outbox/inbox behavior and correlation propagation;
- define configuration validation conventions for policy values.

Exit gate: two representative services can commit an outbox message, consume it
idempotently under tenant isolation, and correlate logs/traces without sharing
tables or user tokens.


[Back to plan](index.md)
