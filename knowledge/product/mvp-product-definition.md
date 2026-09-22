---
type: specification
title: Investor AI MVP Product Definition
description: Approved minimum user value, product operations, and scope boundaries for the Investor AI MVP.
tags: [product, mvp, portfolio, decision-models, scheduler, email]
updated_at: 2026-09-22
status: approved
---

# Investor AI MVP product definition

## Purpose

The MVP is a serious working side project rather than a commercial validation
exercise. It has two goals:

1. provide practical value for managing the owner's investment portfolio;
2. demonstrate a credible, production-minded engineering project that expands
   the owner's technical stack and can be presented publicly or in a resume.

Revenue, pricing, willingness to pay, and product-market fit are not MVP success
criteria.

## Minimum user value

Investor AI must help the user notice a portfolio risk early enough to make a
deliberate decision, or surface a genuinely useful investment opportunity that
the user might otherwise miss.

The minimum product promise is:

> Investor AI analyzes the tenant's portfolio and watchlist in the context of
> that tenant's portfolio-management strategy, identifies positions requiring
> attention and potential opportunities, and delivers the result by email. The
> user remains responsible for every investment decision and action.

A consolidated positions API without strategy-aware analysis is infrastructure,
not the Investor AI product MVP.

## Target user and tenancy

The first user is a self-directed investor managing their own portfolio. The
system remains multitenant: every tenant owns its portfolio data, watchlist,
strategy, analyses, and results.

For the MVP, each tenant's portfolio-management strategy is stored as a Markdown
document. A future design may extract stable parts of that strategy into
structured rules, but a strategy editor or universal rule schema is not required
now.

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

The [MVP architecture](../architecture/mvp-architecture.md) retains the earlier
portfolio-retrieval slice as its first implementation milestone and extends the
system to both independent analyses, a Decision Layer, scheduled execution,
persistent results, and email delivery. The first slice alone is not sufficient
to claim the Investor AI product MVP.

## Deferred decisions

- the detailed structure and semantics of tenant strategy rules;
- exact portfolio-risk signals and thresholds;
- exact opportunity-ranking criteria;
- scheduling cadence for each operation;
- detailed Decision Layer contracts and result schemas;
- whether and when to add Jev, classifiers, rerankers, or LLM-backed decisions;
- richer user interfaces and notification channels;
- expansion beyond a tenant-maintained watchlist.

## Decisions log

| Decision | Resolution | Reason |
| --- | --- | --- |
| Consolidated portfolio alone defines the MVP | Rejected | It is infrastructure and does not yet deliver Investor AI's distinguishing value. |
| Risk detection and opportunity search form one mandatory cycle | Rejected | They are independent operations and may be invoked separately. |
| Compare an existing position with replacements | Deferred | It is not required for the first useful version. |
| Candidate universe | Tenant-maintained watchlist for MVP | Avoids premature whole-market discovery while enabling useful ranking. |
| Tenant strategy representation | Markdown for MVP | Current strategy already exists in this form; structured rules can evolve later. |
| API-only invocation | Rejected as final MVP boundary | Scheduler is required for timely detection; API remains supported. |
| Email only when a signal exists | Rejected | Every analysis sends confirmation, including a no-significant-result outcome. |
| Generic Decision Layer details now | Deferred | Product scope is being defined before detailed architecture. |
| Specialized ML model required | Rejected | Deterministic rules are acceptable initially; future Jev compatibility remains a direction. |
| Commercial validation as success | Out of scope | The project targets personal utility, learning, and a credible public portfolio artifact. |

## Dependency and implementation order

The product-level dependency order is:

```text
Tenant + identity + isolation
            ↓
Portfolio data + market data + watchlist + Markdown strategy
            ↓
Decision-capable portfolio analysis and opportunity search
            ↓
Persistent analysis results
            ↓
API invocation + scheduled invocation
            ↓
Email delivery + observable end-to-end demonstrations
```

Detailed service boundaries and sequencing are intentionally deferred to the
architecture phase.
