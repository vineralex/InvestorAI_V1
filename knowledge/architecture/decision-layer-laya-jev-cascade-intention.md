---
type: concept
title: Laya and Jev Decision Layer Cascade Intention
description: Research direction for bounded typed decisions using Laya locally and Jev as a hosted decision provider or escalation stage.
tags: [architecture, intention, decision-layer, laya, jev, llm, human-in-the-loop]
updated_at: 2026-09-23
status: intention
---

# Laya and Jev Decision Layer cascade

This document preserves a future research direction. It is **not approved
architecture**, is not part of the Portfolio Foundation slice, and does not
change the approved MVP boundaries in [MVP architecture](mvp-architecture.md).

## Hypothesis

A future Decision Layer may evaluate a bounded, typed decision locally with
Laya and escalate cases that do not satisfy a task-specific acceptance policy
to Jev, an LLM, or a human:

```text
Decision request
        |
        v
Laya local decision
        |
        +-- acceptance policy satisfied --> decision
        |
        +-- acceptance policy not satisfied
                         |
                         v
                   Jev / LLM / human
                         |
                         v
                      decision
```

Laya is an open-weight local decision model intended for typed outputs such as
choice, score, and boolean probability rather than prose generation. Jev is a
hosted System One typed-decision provider with compatible concepts: choice,
score, boolean probability, and confidence. An LLM or human may be a later
escalation stage when the task requires reasoning or review beyond those
bounded outputs.

Jev is a first-class candidate, not merely a generic fallback label. Depending
on measured quality, risk, availability, and cost for a particular decision
type, a future policy may use Jev after Laya, use Jev directly, or compare both
providers during evaluation and shadow operation.

## Intended properties

- Prefer fast local evaluation for sufficiently clear, bounded cases.
- Avoid sending every decision input to an external provider.
- Keep the caller independent of the selected local or escalation provider.
- Return one normalized typed decision contract across every stage.
- Support Laya and Jev behind that contract without reducing either provider to
  the other's implementation details.
- Preserve the model, version, policy, evidence references, confidence, route,
  and escalation reason for evaluation and audit.
- Treat thresholds and other operational policy values as configuration backed
  by task-specific validation, not as hardcoded universal constants.
- Allow the escalation chain to vary by decision type and risk.

## Safety boundary

Raw model confidence is not sufficient authority for a financial conclusion or
action. A future acceptance policy must also consider at least:

- the decision type and consequence of an error;
- completeness, provenance, and freshness of controlled application facts;
- whether the model and its confidence are calibrated for that exact task;
- deterministic business constraints and abstention conditions;
- whether human review is mandatory.

The Decision Layer must not authorize access, weaken tenant isolation, invent
financial facts, or place, modify, or cancel orders. These constraints follow
the approved human-control boundary in [MVP architecture](mvp-architecture.md).

## Evidence and cautions

The proposed cascade resembles published Laya/Jev experiments, but the currently
available measurements are early and narrow. One independent experiment found
that a Laya-first cascade could match Jev on a small support-ticket dataset at a
particular threshold, while also reporting that confidence-to-accuracy behavior
was not monotonic. Those results do not establish a threshold or fitness for
Investor AI.

Relevant starting points:

- [Laya project overview](https://laya-ai.com/)
- [TypeSafe Jev overview](https://typesafe.ai/blog/introducing-system-one-models-and-jev)
- [TypeSafe Jev API](https://api.typesafe.ai/docs)
- [Independent Laya/Jev cascade measurements](https://github.com/yibie/laya-jev-lab)

## Required research before approval

1. Define the bounded Investor AI decision types and normalized result contract.
2. Build representative, versioned evaluation datasets for each decision type.
3. Measure accuracy, calibration, abstention quality, latency, resource use,
   privacy exposure, provider availability, and escalation rate separately for
   Laya, Jev, and every other candidate model and language.
4. Compare pure deterministic, pure Laya, pure Jev, LLM-assisted, human-review,
   and cascade policies against the same acceptance criteria.
5. Determine whether confidence is calibrated enough to participate in routing;
   do not assume one global threshold.
6. Define failure, timeout, unavailable-provider, and conflicting-stage behavior.
7. Define what evidence and explanation must accompany an accepted decision.
8. Threat-model prompt/input manipulation, model supply chain, sensitive-data
   exposure, and unsafe automation.
9. Approve cost, deployment, licensing, update, rollback, and observability
   policies before selecting a runtime model.

## Explicit non-decisions

This intention does not select Laya or Jev as the primary provider; establish a
provider order; approve automatic decisions; choose a threshold; define a
runtime topology; or add Decision Layer work to the Portfolio Foundation slice.
