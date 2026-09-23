---
type: specification
title: Portfolio Foundation Service Ownership
description: Service responsibilities, authoritative data, and allowed cross-service references for the Portfolio Foundation slice.
tags: [vertical-slice, services, ownership, data, boundaries]
updated_at: 2026-09-23
status: draft
---

# Service ownership

## Responsibility matrix

| Component | Owns | Does not own |
| --- | --- | --- |
| Tenant Service | Tenant identity and lifecycle; provisioning operation and its projection; tenant status; operator-facing provisioning result | Users, memberships, activation codes, email delivery, broker data, portfolio refresh |
| Identity Service | Users; local credentials; one active tenant membership per user in V1; fixed roles and effective permissions; initial Owner creation; activation code lifecycle; password setup; access and refresh tokens | Tenant lifecycle, email delivery state, broker credentials, portfolio authorization decisions made by other services |
| Email Service | Activation email composition; SMTP delivery through MailKit; attempt/retry state; terminal delivery outcome; service-owned error catalogue for email delivery | User activation state, activation-code validity, tenant status, portfolio data |
| Portfolio Service | Broker connections; encrypted broker credentials; accessible and selected accounts; account position snapshots; connection-validation, initial-import, and refresh operations; portfolio views and freshness | Users, memberships, tenant lifecycle, email, analysis results, trading actions |
| Admin CLI | Authenticated operator interaction with Tenant Service; presentation of provisioning progress | Direct database writes, workflow orchestration, identity or email data ownership |
| Broker adapter | Provider-specific validation, accessible-account discovery, position retrieval, normalization of provider responses and errors | Persistence, user authorization, account selection, refresh lifecycle, consolidation, message publication |

The broker adapter is an internal Portfolio Service boundary, not an independently
deployed Broker Connector Service.

## Data ownership matrix

| Data | Authoritative owner | Tenant-owned | Allowed external reference |
| --- | --- | --- | --- |
| Tenant record and lifecycle | Tenant Service | Yes | `tenantId` |
| Provisioning operation/projection | Tenant Service | Yes | `provisioningOperationId` |
| User and local credential | Identity Service | User scoped within tenant membership | Opaque `userId`; no credential reference |
| Membership, role, effective permissions | Identity Service | Yes | `userId`, `tenantId`; values copied into signed JWT claims |
| Activation challenge | Identity Service | Yes | Opaque delivery reference; plaintext code is never a persistent cross-service reference |
| Email delivery attempt/state | Email Service | Yes | `deliveryId`, correlation and activation delivery reference |
| Broker connection and encrypted credentials | Portfolio Service | Yes | `connectionId`; no secret value |
| Broker account enrollment | Portfolio Service | Yes | `accountId`, provider/external identity held only by Portfolio Service |
| Account position snapshot | Portfolio Service | Yes | `snapshotId` within Portfolio Service contracts |
| Refresh/import operation and errors | Portfolio Service | Yes | `operationId` |
| Local audit record | The service whose action is audited | Usually tenant scoped | Globally unique `auditEventId` and common envelope |
| Outbox/inbox record | Each producing/consuming service | Scoped to the associated message | `messageId`, correlation and causation identifiers |

## Allowed relationships

- Cross-service relationships use opaque identifiers and messages or service
  APIs, never foreign keys or direct database queries.
- A service may retain the foreign identifier needed to interpret a fact it has
  consumed, but it does not become owner of the referenced record.
- JWT claims are signed identity assertions, not replicated Identity Service
  tables. Each receiving API still applies its own permission and resource
  authorization.
- Tenant Service may project onboarding milestones reported by Identity and
  Email without becoming authoritative for users, codes, or deliveries.
- Portfolio Service associates all owned data with `tenantId` and enforces
  tenant isolation independently of the caller.
- Email Service receives only the data required to compose and deliver the
  activation message. It does not query Identity Service storage.

## Prohibited coupling

- shared repositories or shared persistence models;
- a runtime database role reading or writing another service's tables;
- cross-service SQL joins, foreign keys, or transactions;
- using RabbitMQ as the source of identity or authorization truth;
- putting user bearer tokens or broker credentials in messages;
- using Email delivery success as proof that an Owner is active;
- allowing the broker adapter to decide which accounts belong in a portfolio;
- allowing the Admin CLI to repair state by editing PostgreSQL directly.

## Dependency direction

Tenant provisioning initiates Identity work through a targeted command and
observes resulting facts. Identity requests activation delivery from Email.
Portfolio APIs trust locally validated Identity-issued JWTs but do not call
Identity synchronously for each request. Portfolio has no runtime dependency on
Tenant or Email for ordinary connection, refresh, or read operations beyond the
trusted tenant context already present in the JWT.
