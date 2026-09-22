---
type: concept
title: Architectural Intentions
description: Unresolved direction for evolving Investor AI into a multitenant agentic SaaS.
tags: [architecture, saas, multitenancy, microservices]
updated_at: 2026-09-23
status: intention
---

# Architectural intentions

1. Implement Investor AI as a multitenant SaaS service.
2. Replace orchestration inside the C# monolith with choreography between
   microservices.

These are intentions, not approved architecture. They require analysis before
implementation and must not be used to infer missing decisions.

The first architecture pass resolving these intentions is recorded in
[MVP architecture](mvp-architecture.md). That approved document takes
precedence where an earlier intention or supporting note conflicts with it.

The intended supporting concerns are documented separately:

- [Inbound identity and authorization](inbound-identity-and-authorization.md)
- [Multi-tenant data isolation](multi-tenant-data-isolation.md)
- [Agent execution boundaries](agent-execution-boundaries.md)
- [LLM usage accounting](llm-usage-accounting.md)
- [Execution observability](execution-observability.md)
- [SaaS Control Plane](saas-control-plane.md)
- [Outbound identity and authorization](outbound-identity-and-authorization.md)
- [Laya and Jev Decision Layer cascade](decision-layer-laya-jev-cascade-intention.md)
