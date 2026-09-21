---
type: concept
title: Multi-tenant Data Isolation
description: Approved pooled tenancy model and mandatory application and database isolation.
tags: [multitenancy, data, security, authorization]
updated_at: 2026-09-21
status: approved
---

# Model

Investor AI uses a pooled multi-tenant model. Tenants share tables, and every
tenant-owned record contains `tenantId` and is isolated by it.

# Isolation invariant

Tenant filtering must be a mandatory system rule enforced centrally and
automatically in the data-access and authorization layers. It must not depend
on every developer remembering a manual filter; one omission must not permit a
cross-tenant data leak.

The trusted tenant context originates from [inbound identity and
authorization](inbound-identity-and-authorization.md).

# Database enforcement

All tenant-owned tables use PostgreSQL Row-Level Security in addition to
application authorization. Runtime database roles cannot own tenant tables or
bypass RLS. Tenant context is set locally for the current transaction so pooled
connections do not retain another tenant's context.

The MVP uses one database and one schema with strict logical table ownership,
separate service migrations, and separate restricted runtime roles. Services
must not query or join another service's tables. See
[MVP architecture](mvp-architecture.md).
