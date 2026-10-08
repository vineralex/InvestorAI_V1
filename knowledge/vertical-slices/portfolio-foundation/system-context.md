---
type: specification
title: Slice system context
description: Details for slice system context.
tags: [vertical-slice, portfolio, identity, tenancy, alpaca, architecture]
updated_at: 2026-10-09
status: draft
---

# Slice system context

## System context

```mermaid
flowchart LR
    Operator[Operator] --> AdminCLI[Protected Admin CLI]
    AdminCLI --> Tenant[Tenant Service]
    Owner[Owner / Manager / Viewer] --> Identity[Identity Service]
    Owner --> Portfolio[Portfolio Service]

    Tenant <-->|commands and facts| MQ[(RabbitMQ)]
    Identity <-->|commands and facts| MQ
    Email[Email Service] <-->|commands and facts| MQ
    Portfolio <-->|queued work and facts| MQ

    Email --> SMTP[SMTP]
    Portfolio --> Adapter[Broker-neutral adapter boundary]
    Adapter --> Alpaca[Alpaca paper API]

    Tenant --> DB[(One PostgreSQL database and schema)]
    Identity --> DB
    Email --> DB
    Portfolio --> DB

    KeyManager[External key manager in deployed environments] --> Portfolio
    KeyManager --> Identity
    KeyManager --> Email
```

The shared PostgreSQL deployment does not imply shared data ownership. Each
service owns its tables, migrations, runtime role, transactions, audit records,
outbox, and inbox. No service reads or joins another service's tables.

Identity, Tenant, Portfolio, and Email are independently deployable processes.
The initial runtime topology remains Docker Compose on one machine, while the
correctness rules do not assume that a service has only one process instance.


[Back to index](index.md)
