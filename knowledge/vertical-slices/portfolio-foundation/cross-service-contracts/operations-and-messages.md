---
type: specification
title: Operations and message catalogue
description: Details for operations and message catalogue.
tags: [vertical-slice, contracts, messages, outbox, inbox, recovery]
updated_at: 2026-10-09
status: draft
---

# Operations and message catalogue

## Synchronous and asynchronous boundary

A synchronous API response proves only that the owning service has atomically
validated and committed the local acceptance or local state change. It never
claims that a remote service, RabbitMQ consumer, SMTP server, or broker has
already completed later work.

| Conceptual operation | Owner | Synchronous result | Asynchronous continuation |
| --- | --- | --- | --- |
| Create tenant with initial Owner | Tenant | Tenant/provisioning identifiers after local commit | Establish Owner, send activation, project milestones |
| Observe provisioning | Tenant | Current projection and safe errors | None initiated by read |
| Activate initial Owner | Identity | Local activation/password result | Publish Owner-activated fact |
| Sign in / refresh token | Identity | Tokens or safe authentication error | None |
| Create broker connection | Portfolio | Connection/validation operation identifiers after encrypted local commit | Validate connection and obtain accessible accounts |
| Observe connection validation | Portfolio | Current connection/operation state | None initiated by read |
| Select or exclude account | Portfolio | Updated selection and, on inclusion, initial-import identifier | Initial import when changed to included |
| Disconnect broker connection | Portfolio | Terminal disconnect after credentials and inclusion changes commit | Publish resulting facts if needed |
| Request tenant refresh | Portfolio | New or reused operation identifier | Fetch target accounts and aggregate outcomes |
| Observe refresh/import | Portfolio | Current operation, account results, and safe errors | None initiated by read |
| Read portfolio | Portfolio | Account or consolidated view | Never starts refresh |

## Conceptual message catalogue

Names below express meaning only. Final names, versions, and payloads are open
contract-design details.

### Cross-service commands

| Command meaning | Producer | Consumer | Required context |
| --- | --- | --- | --- |
| Establish the initial Owner for a provisioned tenant | Tenant | Identity | Tenant, provisioning operation, initial Owner address/profile, message/correlation/causation, narrow internal authorization |
| Deliver an activation email | Identity | Email | Tenant, delivery and activation references, recipient/template inputs, encrypted activation secret, expiry, correlation context |

Commands have one intended owner/consumer. Consumers validate the internal
JWT issued by Identity Service, its recipient/audience, and its action scope;
RabbitMQ delivery alone grants no authority.

### Cross-service facts

| Fact meaning | Producer | Interested consumers |
| --- | --- | --- |
| Tenant created | Tenant | Identity and future interested services where justified |
| Initial Owner established | Identity | Tenant |
| Initial Owner establishment failed terminally | Identity | Tenant |
| Activation delivery succeeded | Email | Tenant and Identity where delivery projection is required |
| Activation delivery failed terminally | Email | Tenant and Identity where resend/action state is required |
| Initial Owner activated | Identity | Tenant |
| Portfolio refresh finished | Portfolio | No mandatory consumer in this slice; published contract may support later Scheduler/Analysis integration |

Facts describe completed local truth and are not requests for another service to
pretend that truth exists.

### Portfolio-internal queued work and facts

These messages remain within the Portfolio Service ownership boundary even when
RabbitMQ separates API and worker processes:

- validate broker connection;
- validation succeeded or failed;
- execute initial account import;
- execute tenant refresh;
- fetch positions for one account;
- account import succeeded or failed;
- tenant refresh completed, partially completed, or failed.

They follow the same message identity, inbox, outbox, retry, and authorization
rules as cross-service work. They do not create a new service boundary or new
data owner.


[Back to index](index.md)
