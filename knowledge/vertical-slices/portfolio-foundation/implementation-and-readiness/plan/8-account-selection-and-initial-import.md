---
type: specification
title: 8. Account selection and initial import
description: Scope and exit gate for this implementation milestone.
tags: [vertical-slice, implementation-order, testing, readiness, acceptance]
updated_at: 2026-10-09
status: draft
---

# 8. Account selection and initial import


- implement explicit `included` selection;
- automatically create initial-import work on inclusion;
- guarantee one in-flight fetch per account;
- atomically commit complete empty or nonempty snapshots;
- preserve prior snapshots on failed work.

Exit gate: selecting a real Alpaca paper account eventually makes its real
positions visible, and selecting an empty account yields a valid empty snapshot.


[Back to plan](index.md)
