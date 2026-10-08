---
type: specification
title: Product operations and delivery
description: Details for product operations and delivery.
tags: [product, mvp, portfolio, decision-models, scheduler, email]
updated_at: 2026-10-09
status: approved
---

# Product operations and delivery

## Independent product operations

The MVP exposes two independent operations. Neither operation is a mandatory
step in a workflow initiated by the other.

### Portfolio attention analysis

Analyze current portfolio positions against the tenant's strategy and current
market facts. Identify positions that require the user's attention early enough
to reduce the chance of an avoidable loss.

The operation informs the user; it does not decide that a position must be sold.

### Opportunity search

Analyze and rank potential opportunities according to the tenant's strategy.
For the MVP, the candidate universe is the tenant's user-maintained watchlist.
Scanning the entire market is deferred.

The operation returns several candidates rather than selecting an asset for the
user to buy.

### Explicitly excluded operation

Pairwise comparison between an existing position and possible replacements is
not required for the MVP.

## Invocation and delivery

Both analysis operations can be invoked independently through:

- an API request;
- a scheduler.

Scheduling is part of the MVP because manual analysis alone does not solve the
original problem of noticing adverse movement too late. API invocation remains
available for manual and demonstrative use.

Every completed analysis sends an email, regardless of whether it was triggered
by the scheduler or the API. An analysis that finds no significant result still
sends an explicit "nothing significant found" outcome. Results are also
available through the API.

Email is the only required outbound user channel. A Web UI is not part of the
MVP.

## Decision Models direction

The MVP includes a distinct Decision Layer for bounded decisions such as a
choice, score, ranking, or probability. The intended separation is:

- LLMs handle reasoning support, explanations, and language generation;
- decision models handle bounded classification, ranking, scoring, or choice.

The Decision Layer must be designed generally enough not to prevent a future
Jev or equivalent specialized implementation. The MVP does not require a Jev
integration or another specialized ML model. A hard-assigned implementation
using deterministic rules is acceptable for the first version.

This document intentionally does not prescribe detailed interfaces, result
schemas, provider factories, runtime selection, or model orchestration. Those
belong to a later architecture design.

Decision models must not make authorization or security decisions. They must
not be the sole guardrail before a real financial action.


[Back to index](index.md)
