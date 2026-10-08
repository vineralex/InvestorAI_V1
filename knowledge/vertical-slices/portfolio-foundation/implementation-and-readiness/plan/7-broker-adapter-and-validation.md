---
type: specification
title: 7. Broker adapter and validation
description: Scope and exit gate for this implementation milestone.
tags: [vertical-slice, implementation-order, testing, readiness, acceptance]
updated_at: 2026-10-09
status: draft
---

# 7. Broker adapter and validation


- define neutral validation/account/position/error contract;
- adapt the relevant prior Alpaca normalization behavior only after review;
- implement asynchronous connection-validation operation;
- persist accessible accounts without assuming universal cardinality.

Exit gate: valid, rejected, temporarily unavailable, repeated, and malformed
provider outcomes produce the correct connection states and stable errors.


[Back to plan](index.md)
