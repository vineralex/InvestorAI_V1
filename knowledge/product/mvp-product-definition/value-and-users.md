---
type: specification
title: Product value and users
description: Details for product value and users.
tags: [product, mvp, portfolio, decision-models, scheduler, email]
updated_at: 2026-10-09
status: approved
---

# Product value and users

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


[Back to index](index.md)
