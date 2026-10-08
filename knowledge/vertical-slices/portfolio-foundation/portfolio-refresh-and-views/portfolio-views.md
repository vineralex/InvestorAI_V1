---
type: specification
title: Portfolio views and availability
description: Details for portfolio views and availability.
tags: [vertical-slice, portfolio, refresh, snapshots, freshness, consolidation]
updated_at: 2026-10-09
status: draft
---

# Portfolio views and availability

## Portfolio availability

| Status | Meaning |
| --- | --- |
| `NotConfigured` | No account currently has `included = true`. |
| `Ready` | Every included account has a `Fresh` successful snapshot. |
| `Degraded` | At least one included account has a usable snapshot and not every included account is `Fresh`; one or all accounts may be stale and others may be unavailable. |
| `Unavailable` | Included accounts exist, but none has a usable successful snapshot. |

At `Degraded`, calculations use only available snapshots and must disclose the
included, represented, stale, and missing account sets. A successful empty
snapshot participates normally and is not confused with missing data.

Freshness is evaluated per account. A successful import starts as `Fresh` and
ages into `Stale` after a configured threshold. A later failed refresh makes the
prior snapshot `Stale` immediately, even if it is younger than the age threshold.

## Required read views

Both views are required in the slice and are derived from the same account
snapshots:

### Account view

Positions remain grouped by broker connection and account. The response includes
account inclusion, data status, snapshot time, refresh participation, and safe
errors without credential details.

### Consolidated view

Positions representing the same reliably identified instrument and currency are
aggregated across included accounts. The response preserves the contributing
account breakdown. A matching display ticker alone is not enough to establish
instrument identity; ambiguous positions remain separate.

Every response includes:

- portfolio availability status;
- response-generation time;
- current refresh operation when one exists;
- included-account coverage;
- per-account snapshot times and data statuses;
- stale/missing disclosures and safe error codes;
- positions for the selected view.

Reading a portfolio never starts refresh automatically.

## Currency policy

- Every position displays the currency reported for that account position.
- The slice does not add monetary values across currencies.
- It does not calculate totals grouped by currency.
- It does not define a base currency or perform FX conversion.
- Positions with different currencies are not combined into one consolidated
  position.

## Read authorization

Owner, Manager, and Viewer may read both views, freshness, coverage, and safe
refresh state. Owner and Manager may request refresh. Viewer cannot request it.
Internal diagnostics, credentials, secret metadata, and unsafe provider errors
are never returned by either view.

[Back to index](index.md)
