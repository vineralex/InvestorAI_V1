---
type: concept
title: Agent Execution Boundaries
description: Intended responsibilities of Agent Runtime, Agent Gateway, Memory Service, and Domain Services.
tags: [agents, runtime, gateway, memory, services]
updated_at: 2026-09-21
status: intention
---

# Components

- **Agent Runtime** is the execution environment for agent workflows and
  multi-agent orchestration.
- **Agent Gateway** is the controlled access point from agents to tools, APIs,
  and external resources.
- **Memory Service** owns agent state and memory, including
  `tenant/user/session` scoping, short-term and long-term memory, retrieval,
  summarization, retention, and lifecycle.
- **Domain services** own business data and the rules governing it.

# Boundaries

Agent Gateway controls calls to tools, APIs, and external resources. Domain
data is accessed through the relevant domain/data services.

Each service independently applies authorization using the trusted tenant/user
context described in [inbound identity and
authorization](inbound-identity-and-authorization.md).

```text
Agent Runtime
   ├──> Agent Gateway
   │       ├──> Domain/Data Services ──> DB
   │       └──> External Tools / APIs
   │
   └──> Memory Service ──> Memory Storage
```

External calls additionally follow [outbound identity and
authorization](outbound-identity-and-authorization.md).

