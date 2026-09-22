---
okf_version: "0.2"
title: Investor AI knowledge
description: Entry point for the Investor AI Open Knowledge Format bundle.
---

# Start here

This directory is an Open Knowledge Format (OKF) v0.2 bundle. When creating or
editing knowledge:

- use UTF-8 Markdown with YAML frontmatter and follow the
  [OKF specification](https://github.com/GoogleCloudPlatform/open-knowledge-format/blob/main/SPEC.md);
- give every concept document a non-empty `type` and provide a concise `title`,
  `description`, relevant `tags`, `updated_at`, and lifecycle `status`;
- use `index.md` files for progressive disclosure and standard Markdown links
  between related concepts;
- preserve unknown OKF frontmatter fields when editing existing documents.

Read one matching concept and follow its links only as needed. Documents marked
as `status: intention` describe direction to investigate, not approved
architecture.

- Product direction and unresolved architecture: [architectural intentions](architecture/intentions.md)
- Approved product value and scope of the MVP: [MVP product definition](product/mvp-product-definition.md)
- Approved MVP boundaries and decisions: [MVP architecture](architecture/mvp-architecture.md)
- User identity, JWT, RBAC, and ABAC: [inbound identity and authorization](architecture/inbound-identity-and-authorization.md)
- Tenant-owned data and isolation: [multi-tenant data isolation](architecture/multi-tenant-data-isolation.md)
- Agent Runtime, Gateway, Memory, and domain boundaries: [agent execution boundaries](architecture/agent-execution-boundaries.md)
- LLM token usage and cost facts: [LLM usage accounting](architecture/llm-usage-accounting.md)
- Logs, metrics, and traces: [execution observability](architecture/execution-observability.md)
- Tenant onboarding and lifecycle: [SaaS Control Plane](architecture/saas-control-plane.md)
- External credentials and delegated actions: [outbound identity and authorization](architecture/outbound-identity-and-authorization.md)
- Future typed-decision routing: [Laya and Jev Decision Layer cascade](architecture/decision-layer-laya-jev-cascade-intention.md)
