---
type: concept
title: MVP slices and open questions
description: Details for mvp slices and open questions.
tags: [architecture, mvp, multitenancy, portfolio, analysis, scheduling, email]
updated_at: 2026-10-09
status: approved
---

# MVP slices and open questions

## Implementation slices

1. **Portfolio foundation.** Provision tenant and owner, activate with a
   one-time email code and password setup through the API, sign in, connect
   Alpaca paper accounts, import positions asynchronously,
   expose the consolidated portfolio, and support manual tenant refresh. This
   is the previously approved first slice, not the complete product MVP.
2. **Portfolio attention end to end.** Add Markdown strategy, controlled market
   facts, Analysis Service with its initial Decision Layer, persisted runs and
   results, API initiation and retrieval, scheduled initiation, and email for
   every completed attention analysis, including empty outcomes.
3. **Opportunities end to end.** Add tenant watchlist and candidate facts; use
   the same initiation, result, Decision Layer, scheduling, and email boundaries
   for independent watchlist ranking.
4. **MVP verification.** Demonstrate both API and scheduled paths end to end,
   tenant isolation, idempotent retries, data-freshness disclosure, result
   retrieval, and email delivery with observable failure states.

## Open questions

- Strategy interpretation, portfolio-risk signals, thresholds, opportunity
  criteria, and ranking semantics.
- Market-data provider and refresh/freshness policy, including minimum usable
  coverage for a completed analysis.
- Schedule cadence and tenant configuration for each operation and refresh.
- Detailed Decision Layer contracts, result shapes, and any future Jev or
  equivalent integration.
- Email recipients and escalation after persistent delivery failure.

These questions do not alter the approved product boundary. The MVP excludes
automatic trading, a Web UI, whole-market scanning, pairwise replacement
comparison, mandatory specialized ML, billing, realtime broker synchronization,
database-per-service, and Kubernetes. Agent Runtime, Agent Gateway, and Memory
Service remain [architectural intentions](../intentions.md), not MVP dependencies.

[Back to index](index.md)
