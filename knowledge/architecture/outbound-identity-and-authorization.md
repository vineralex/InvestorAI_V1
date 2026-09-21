---
type: concept
title: Outbound Identity and Authorization
description: Intended identity chain, credential boundary, and policy checks for external actions.
tags: [identity, authorization, credentials, delegation, audit]
updated_at: 2026-09-21
status: intention
---

# Identity distinction

Investor AI distinguishes the identity executing a workload from the identity
on whose behalf an external action is performed.

- **Workload identity** identifies the executing service or agent, for example
  `InvestorAI-ResearchAgent`, and is used for service-to-service access when
  the workload acts on its own behalf.
- **Delegated user identity** is used when Investor AI accesses an external
  system on behalf of a user, such as Gmail, Google Drive, Microsoft 365, or a
  broker API. External credentials remain scoped to their originating
  `tenantId` and `userId`.

# Credential boundary

OAuth tokens, API keys, and service credentials are managed by a dedicated
credential/identity layer and are never exposed directly to the LLM or agent
code.

Authentication and authorization for outbound calls are evaluated separately.
Valid external credentials do not authorize every action.

# High-risk actions

High-risk actions, including sending messages, modifying external data,
transferring money, or executing trades, require action-specific policy checks
and, when appropriate, explicit user approval or step-up authentication.

# Audit chain

Every outbound action preserves an auditable identity chain:

```text
tenantId → userId → workload/agent → delegated identity → action → external resource
```

Example:

```text
tenant-A → user-123 → TradingAgent → user's broker account → BUY 10 AAPL → Broker API
```

The audit record distinguishes:

- `performedBy`: the workload or agent that executed the action;
- `onBehalfOf`: the user whose authority was delegated;
- the authorization or approval that permitted the action.

The inbound tenant/user context is defined in [inbound identity and
authorization](inbound-identity-and-authorization.md). The executing component
is defined by [agent execution boundaries](agent-execution-boundaries.md).
