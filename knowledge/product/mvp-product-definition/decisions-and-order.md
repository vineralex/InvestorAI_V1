---
type: specification
title: Product decisions and delivery order
description: Details for product decisions and delivery order.
tags: [product, mvp, portfolio, decision-models, scheduler, email]
updated_at: 2026-10-09
status: approved
---

# Product decisions and delivery order

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

[Back to index](index.md)
