---
type: concept
title: SaaS Control Plane
description: Approved MVP responsibility for tenant provisioning and lifecycle.
tags: [saas, control-plane, tenants, provisioning]
updated_at: 2026-09-21
status: approved
---

# Responsibility

Tenant Service owns tenant onboarding and lifecycle independently from portfolio
runtime data.

For the first vertical slice an operator uses a protected Admin CLI. The CLI is
a thin client of Tenant Service and never writes directly to PostgreSQL. Tenant
Service creates the tenant and publishes the tenant-created fact and the request
to establish the initial owner. Identity Service creates a pending owner, owns
the one-time activation code, and requests delivery from Email Service.

Self-service signup, invitations, and the broader control plane are deferred.
See [MVP architecture](mvp-architecture.md).
