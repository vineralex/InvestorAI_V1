---
type: concept
title: Execution Observability
description: Intended correlation context for agent and workflow execution.
tags: [observability, logging, metrics, tracing, agents]
updated_at: 2026-09-21
status: intention
---

# Correlation context

Every agent or workflow execution carries:

- `tenantId`;
- `userId`;
- `runId`;
- `agentId`.

This context must be present in logs, metrics, and distributed traces for the
entire execution.

The tenant and user values come from the trusted context defined by [inbound
identity and authorization](inbound-identity-and-authorization.md). Async
messages additionally carry message, correlation, and causation identifiers.
Portfolio refresh exposes an operation identifier so an API caller can observe
its asynchronous completion. Exact telemetry and operation schemas are deferred
to service design. LLM calls later record their own
[usage facts](llm-usage-accounting.md).
