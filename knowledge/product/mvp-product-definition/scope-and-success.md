---
type: specification
title: Product scope and success
description: Details for product scope and success.
tags: [product, mvp, portfolio, decision-models, scheduler, email]
updated_at: 2026-10-09
status: approved
---

# Product scope and success

## Human control and safety boundary

Investor AI provides decision support only:

- it may identify a position requiring attention;
- it may rank investment candidates;
- it may explain the facts and tenant strategy relevant to a result;
- it does not place, modify, or cancel orders;
- it does not automatically remove or acquire a position;
- it does not make the user's final investment decision.

All financial calculations must be based on application-controlled market and
portfolio facts. Generated prose must not become the authoritative source of
financial numbers.

## MVP scope

### Included

- multitenant ownership and isolation of relevant data;
- portfolio connection and current portfolio data;
- a Markdown portfolio-management strategy per tenant;
- a tenant-maintained watchlist;
- portfolio attention analysis;
- watchlist opportunity search and ranking;
- a Decision Layer with a first deterministic implementation allowed;
- independent API invocation of both operations;
- independent scheduled invocation of both operations;
- email after every completed analysis;
- API access to analysis results;
- sufficient auditability and observability to demonstrate a serious working
  system.

### Excluded

- automatic order execution;
- autonomous buy or sell decisions;
- pairwise comparison of current positions and replacement candidates;
- whole-market opportunity scanning;
- Web UI;
- a universal structured strategy language or strategy editor;
- a mandatory specialized ML decision model;
- detailed generic Decision Layer architecture;
- billing, pricing, or other commercial-model work.

## Acceptance and success criteria

The MVP is complete only when both independent operations work end to end via
API and scheduler and send an email after every completed run.

The MVP demonstrates personal value when at least one of the following occurs:

1. it identifies a portfolio risk in time to be useful;
2. it surfaces a genuinely useful candidate opportunity;
3. it contributes materially to a deliberate investment decision.

One such result is sufficient for the initial technical pilot. It does not claim
statistical investment performance or commercial validation.

For public demonstration, the project must show real end-to-end behavior rather
than disconnected architectural patterns. Documentation may describe the
architecture and replaceable components, but the portfolio analysis,
opportunity search, scheduling, API, and email paths must actually run.

## Relationship to the approved architecture

The [MVP architecture](../../architecture/mvp-architecture/index.md) retains the earlier
portfolio-retrieval slice as its first implementation milestone and extends the
system to both independent analyses, a Decision Layer, scheduled execution,
persistent results, and email delivery. The first slice alone is not sufficient
to claim the Investor AI product MVP.


[Back to index](index.md)
