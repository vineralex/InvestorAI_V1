---
type: specification
title: Cross-service sequences
description: Details for cross-service sequences.
tags: [vertical-slice, contracts, messages, outbox, inbox, recovery]
updated_at: 2026-10-09
status: draft
---

# Cross-service sequences

## End-to-end sequences

### Provision tenant and activate Owner

```mermaid
sequenceDiagram
    actor Operator
    participant CLI as Admin CLI
    participant Tenant as Tenant Service
    participant MQ as RabbitMQ
    participant Identity as Identity Service
    participant Email as Email Service
    participant SMTP
    actor Owner

    Operator->>CLI: Create tenant + initial Owner
    CLI->>Tenant: Authenticated provisioning request
    Tenant->>Tenant: Commit tenant, operation, audit, outbox
    Tenant-->>CLI: provisioningOperationId
    Tenant->>MQ: EstablishInitialOwner
    MQ->>Identity: Deliver command at least once
    Identity->>Identity: Create pending Owner + activation challenge atomically
    Identity->>MQ: InitialOwnerEstablished + DeliverActivation
    MQ->>Tenant: Project Owner milestone
    MQ->>Email: Deliver encrypted activation payload
    Email->>SMTP: Send activation email
    Email->>MQ: DeliverySucceeded or terminal DeliveryFailed
    MQ->>Tenant: Project delivery milestone
    Owner->>Identity: Submit code + password
    Identity->>Identity: Activate atomically and consume code
    Identity->>MQ: OwnerActivated
    MQ->>Tenant: Mark tenant Active and provisioning complete
```

### Connect account and import initial positions

```mermaid
sequenceDiagram
    actor Owner
    participant API as Portfolio API
    participant DB as Portfolio PostgreSQL ownership
    participant Publisher as Portfolio Outbox Publisher
    participant MQ as RabbitMQ
    participant Worker as Portfolio Worker
    participant Adapter as Broker Adapter
    participant Broker

    Owner->>API: Create connection with credentials
    API->>DB: Encrypt and commit connection, operation, audit, outbox
    API-->>Owner: connectionId + validationOperationId
    Publisher->>DB: Load committed outbox work
    Publisher->>MQ: Publish validation work
    MQ->>Worker: Validate connection
    Worker->>Adapter: Validate and obtain accessible accounts
    Adapter->>Broker: Provider-specific read calls
    Broker-->>Adapter: Access/account result
    Adapter-->>Worker: Neutral result
    Worker->>DB: Commit Ready or safe failure state
    Owner->>API: Set selected account included=true
    API->>DB: Commit selection, import operation, audit, outbox
    API-->>Owner: initialImportOperationId
    Publisher->>DB: Load committed outbox work
    Publisher->>MQ: Publish account import work
    Worker->>Adapter: Fetch complete positions
    Adapter->>Broker: Read positions
    Worker->>DB: Atomically commit complete account snapshot and result
    Owner->>API: Read portfolio
    API-->>Owner: Positions, coverage, freshness, and status
```

### Authenticate and call a protected API

```mermaid
sequenceDiagram
    actor User as Activated user
    participant Identity as Identity Service
    participant Portfolio as Portfolio Service

    User->>Identity: Submit local credentials
    Identity->>Identity: Validate user, membership, role, and tenant eligibility
    Identity-->>User: Short-lived access JWT + rotated refresh token
    User->>Portfolio: Request with access JWT
    Portfolio->>Portfolio: Validate signature, issuer, audience, lifetime, permissions, and tenant/resource attributes
    Portfolio-->>User: Authorized result or safe denial
```

Portfolio does not call Identity synchronously during this request, and the
user bearer token is not forwarded into asynchronous work.

### Manual tenant refresh

```mermaid
sequenceDiagram
    actor User as Owner or Manager
    participant API as Portfolio API
    participant DB as Portfolio PostgreSQL ownership
    participant Publisher as Portfolio Outbox Publisher
    participant MQ as RabbitMQ
    participant W1 as Portfolio Worker instance A
    participant W2 as Portfolio Worker instance B
    participant Broker

    User->>API: Request tenant refresh
    API->>DB: Atomically create or find unfinished/recent operation
    DB-->>API: operationId + created/reused
    API-->>User: Accepted operation
    Publisher->>DB: Load committed outbox work when newly created
    Publisher->>MQ: Publish refresh work
    MQ->>W1: Execute refresh
    W1->>DB: Atomically claim Pending operation
    W2->>DB: Competing/repeated claim
    DB-->>W2: Already claimed; do not execute twice
    par Account work up to configured limit
        W1->>Broker: Fetch account A positions
    and
        W1->>Broker: Fetch account B positions
    end
    W1->>DB: Commit each account snapshot/result independently
    W1->>DB: Commit aggregate terminal outcome and audit/outbox
    User->>API: Observe operation / read portfolio
    API-->>User: Status, per-account results, errors, snapshots, freshness
```


[Back to index](index.md)
