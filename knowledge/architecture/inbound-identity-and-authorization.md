---
type: concept
title: Inbound Identity and Authorization
description: Approved identity, tenant membership, and authorization boundaries for the MVP.
tags: [identity, authentication, authorization, jwt, rbac, abac]
updated_at: 2026-09-21
status: approved
---

# Authentication

Identity Service is the MVP identity provider. It uses ASP.NET Core Identity
and OpenIddict, stores local credentials, and issues standards-based JWTs.

Example claims:

```text
userId = 123
tenantId = company-a
role = manager
```

The JWT contains trusted SaaS identity context, including:

- `userId`;
- `tenantId`;
- roles/scopes;
- attributes required for authorization.

# Context propagation

Each HTTP API validates JWT signature, issuer, audience, lifetime, and required
permissions locally. It does not make a synchronous authorization round trip to
Identity Service for every request.

User bearer tokens terminate at the HTTP boundary. Async commands carry a
separate, narrowly scoped internal JWT issued by Identity Service for the target
service and action. RabbitMQ is treated as transport rather than an identity
authority. Detailed token and message schemas are deferred to service design.

The LLM or agent is never a trusted source of identity or authorization data.

# Authorization model

The model is **RBAC + tenant/resource attributes**:

- RBAC governs role-based permissions.
- ABAC applies tenant- and resource-level restrictions based on attributes.

Tenant scoping is also enforced at the data boundary; see
[multi-tenant data isolation](multi-tenant-data-isolation.md).

The MVP roles and their capabilities are defined in
[MVP architecture](mvp-architecture.md).
