---
type: concept
title: MVP Architecture
description: Approved first-pass architecture and first vertical slice for Investor AI V1.
tags: [architecture, mvp, saas, multitenancy, microservices]
updated_at: 2026-09-21
status: approved
---

# MVP architecture

This document records the decisions approved during the first architecture
pass. It defines boundaries and invariants, not detailed service designs.

## Product boundary

Investor AI V1 is a multitenant SaaS for portfolio management. A tenant is a
company. For V1 a user has one active tenant membership, while the data model
keeps identity and membership separate so this restriction can change later.

A tenant can connect multiple broker accounts. Broker connections and accounts
belong to the tenant, not to an individual user. All active accounts contribute
to one logical consolidated portfolio; account-level positions remain available
internally as its source data.

## First vertical slice

The first vertical slice ends after the following flow:

1. An operator creates a tenant and its initial owner through an Admin CLI.
2. The owner receives a one-time activation code by email and sets a password
   through the API.
3. The owner signs in and receives a JWT.
4. The owner connects one or more Alpaca paper accounts.
5. Positions are imported asynchronously.
6. An authorized user reads the tenant's consolidated portfolio through the API.
7. An Owner or Manager can request a manual portfolio refresh.

The Admin CLI is a thin authenticated client of Tenant Service. It never writes
directly to the database.

## Services

The initial system contains independently deployable services:

- Identity Service;
- Tenant Service;
- Portfolio Service;
- Email Service.

It also uses RabbitMQ and PostgreSQL. Admin CLI is a client, not a service that
owns data.

Identity Service uses .NET, ASP.NET Core Identity, and OpenIddict. It owns users,
passwords, memberships, roles, activation codes, and token issuance. Tenant
Service owns tenant lifecycle. Portfolio Service owns broker connections,
broker accounts, position snapshots, refresh operations, and the consolidated
portfolio. Email Service initially supports only owner-activation email and
sends through SMTP using MailKit.

Alpaca paper is the only broker provider in the first slice. Its adapter stays
inside Portfolio Service, behind a provider-neutral boundary. There is no
Broker Connector Service or realtime broker listener in the MVP.

## Choreography

Cross-service workflows use RabbitMQ and choreography. Services publish facts
or requests and react independently; there is no central component that calls
every step of a workflow.

RabbitMQ is transport, not an identity authority. Commands carry a narrowly
scoped internal JWT issued by Identity Service. The receiving service validates
its signature, issuer, audience, scope, tenant, lifetime, message identity, and
binding to the command. A user's bearer token is validated at the HTTP boundary
and is never placed on the message bus.

Delivery is at least once. Consumers and state changes must therefore be
idempotent. Services use transactional outbox/inbox patterns and retain message,
correlation, and causation identifiers. Exact message schemas are deferred to
service design.

## Portfolio refresh

Portfolio Service does not care what triggered a refresh. A user, a future
scheduler, or another authorized component can request the same tenant-level
operation. The public operation refreshes all active accounts for the tenant;
refreshing a single account is an internal technical operation.

Only one refresh may run for a tenant at a time. Concurrent requests join the
active operation instead of starting competing imports.

Refresh is allowed to succeed partially. Successfully read accounts receive new
snapshots. An unavailable account retains its last successful snapshot and is
marked stale. The consolidated portfolio exposes whether it is ready, degraded,
or failed and never presents stale data as fresh.

The API accepts a refresh asynchronously and exposes a refresh-operation
identifier so callers can observe completion. Exact endpoints and state names
are deferred to Portfolio Service design.

There is no scheduled or realtime synchronization in the first slice. A future
Scheduler Service may publish the same refresh request without changing
Portfolio Service. Whether that service internally uses Quartz is deliberately
deferred.

## Authorization

Authorization combines tenant membership with three roles:

| Capability | Owner | Manager | Viewer |
| --- | --- | --- | --- |
| View the portfolio | Yes | Yes | Yes |
| Request a portfolio refresh | Yes | Yes | No |
| Manage broker connections | Yes | No | No |
| Manage tenant users and roles | Yes | No | No |
| Manage the tenant | Yes | No | No |
| Future strategy and analysis actions | Yes | Yes | No |

A tenant must always have at least one active Owner. Its final Owner cannot be
removed, deactivated, or demoted.

## Data architecture and isolation

The MVP uses one PostgreSQL database and one schema. Service ownership is still
strict:

- each table has one owning service;
- a service never queries, joins, or writes another service's tables;
- services do not share repositories, foreign keys, or database transactions;
- each service owns its migrations and migration journal;
- separate runtime database roles receive access only to their service's tables.

Tenant-owned tables include `tenant_id`. Tenant isolation is enforced both by
application authorization and PostgreSQL Row-Level Security. Migration roles
own tables and policies; runtime roles cannot bypass RLS. Tenant context is set
transaction-locally for database work. Cross-tenant isolation requires
integration tests.

This logical ownership is required so a service can move to its own database in
the future, but database-per-service is not part of the MVP.

## Broker credentials

Portfolio Service stores broker credentials encrypted for the MVP. Encryption
keys live outside the database and repository. Plaintext credentials exist only
while needed in memory and must never appear in messages or logs. A separate
credential service is deferred.

## Deployment

Each service runs as a separate process and container. Local and initial MVP
deployment uses Docker Compose on one machine with RabbitMQ and PostgreSQL.
Kubernetes, high availability, and distributed production topology are outside
this architecture pass.

## Migration from Investor AI

Investor AI V1 is a new system, not a structural migration of the old
application.

- Existing domain logic is not copied as-is; each rule must be reviewed and
  adapted before use.
- The existing database schema is not a foundation for the V1 schema.
- The old Worker, service boundaries, and Quartz setup are not migrated.
- Existing Alpaca code is reference material only until reviewed.
- Existing tests are not automatically authoritative because they may encode
  the old model.
- The email implementation is the only current reuse candidate, and it must be
  adapted to the independent Email Service and activation-only scope.

## Explicitly deferred

The first vertical slice excludes:

- AI, agents, memory, and briefings;
- investment strategy and recommendations;
- Scheduler Service and automatic runs;
- Web UI;
- invitations for additional users;
- billing and subscriptions;
- broker providers other than Alpaca;
- live trading and autonomous order execution;
- realtime broker synchronization;
- database-per-service;
- Kubernetes and production high availability.

