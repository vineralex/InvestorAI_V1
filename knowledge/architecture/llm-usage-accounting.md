---
type: concept
title: LLM Usage Accounting
description: Intended usage facts and separation of agent execution from cost calculation.
tags: [llm, usage, cost, accounting]
updated_at: 2026-09-21
status: intention
---

# Usage fact

Every LLM call records usage context as a separate consumption fact:

- `tenantId`;
- `userId`;
- `workflow/run`;
- `agent`;
- `provider/model`;
- `input tokens`;
- `output tokens`.

# Cost responsibility

Cost is calculated from these facts independently of the agent. The agent
performs its task but does not own cost-calculation logic.

The identifiers used here align with the context described in [execution
observability](execution-observability.md).

