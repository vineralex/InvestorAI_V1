---
type: specification
title: 9. Refresh orchestration and snapshots
description: Scope and exit gate for this implementation milestone.
tags: [vertical-slice, implementation-order, testing, readiness, acceptance]
updated_at: 2026-10-09
status: draft
---

# 9. Refresh orchestration and snapshots


- implement database-coordinated create-or-find across Portfolio instances;
- capture target account set at acceptance;
- apply unfinished/recent-operation reuse policy;
- run account work with bounded parallelism and transient retry;
- aggregate success/partial/failure with per-account error lists;
- recover expired claims and terminate exhausted work safely.

Exit gate: concurrency, redelivery, process death, partial provider failure, and
all-account failure tests preserve invariants and never erase successful data.


[Back to plan](index.md)
