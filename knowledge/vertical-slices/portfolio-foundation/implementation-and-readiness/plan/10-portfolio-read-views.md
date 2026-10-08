---
type: specification
title: 10. Portfolio read views
description: Scope and exit gate for this implementation milestone.
tags: [vertical-slice, implementation-order, testing, readiness, acceptance]
updated_at: 2026-10-09
status: draft
---

# 10. Portfolio read views


- implement `NotConfigured`, `Ready`, `Degraded`, and `Unavailable` calculation;
- expose account and consolidated views over the same snapshots;
- disclose coverage, per-account freshness, mixed snapshot times, active refresh,
  and safe errors;
- preserve currency per position without FX or cross-currency totals.

Exit gate: every accepted view is reproducible from committed snapshots and
cannot misrepresent stale/missing coverage as current or complete.


[Back to plan](index.md)
