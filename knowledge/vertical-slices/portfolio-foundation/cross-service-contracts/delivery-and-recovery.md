---
type: specification
title: Message delivery and recovery
description: Details for message delivery and recovery.
tags: [vertical-slice, contracts, messages, outbox, inbox, recovery]
updated_at: 2026-10-09
status: draft
---

# Message delivery and recovery

## Message identity and correlation

Every asynchronous envelope carries:

- unique `messageId` for transport deduplication;
- `tenantId` where the work is tenant scoped;
- relevant business `operationId`;
- `correlationId` for the end-to-end request/workflow;
- `causationId` identifying the command/event that produced this message;
- producer, message type, contract version, and occurrence time;
- narrow internal authorization context where the message is a command.

`messageId` and `operationId` are deliberately different. A single operation
may produce many messages, attempts, and facts.

## Local transaction pattern

For each service-owned transition:

1. validate current state and authorization;
2. update service-owned business data;
3. append the service-owned local audit record;
4. add any outgoing message to the service outbox;
5. commit all four in one local transaction.

An outbox publisher retries delivery until RabbitMQ accepts the message. A
consumer validates the message, checks/inserts its inbox identity, applies its
business transition, appends audit, and writes resulting outbox messages in one
local transaction. RabbitMQ is acknowledged only after commit.

No workflow uses a transaction across services, databases roles, RabbitMQ,
SMTP, or a broker API.

## Duplicate, ordering, and stale-message behavior

- The consumer inbox prevents executing the same `messageId` twice.
- Business state transitions also enforce operation-level idempotency so two
  different messages cannot repeat a completed business action.
- Repeating an already accepted create-tenant or create-connection request must
  return/recover the original accepted result rather than create a second
  business resource. The exact request-identity mechanism for the case where
  the first HTTP response was lost remains a contract-design question.
- Consumers reject or harmlessly ignore transitions that are invalid for the
  current state; they do not roll state backward.
- Facts may arrive after the caller has timed out or after a newer read; the
  authoritative service state remains the source of truth.
- Contract versions are explicit. Unsupported versions are not guessed at or
  deserialized into a superficially similar shape.

## Dead-letter and stuck-work policy

- Transient transport/processing failures are retried with configured limits
  and delays.
- After attempts are exhausted, an unprocessable message moves to a dead-letter
  queue.
- The service records a safe audit event, metric, and alert without exposing
  secret payloads.
- A recovery mechanism prevents the associated operation from remaining
  indefinitely `Pending` or `Running`; it records terminal `Failed` state and
  stable errors when safe automated recovery is exhausted.
- Messages are not automatically replayed from the dead-letter queue. An
  operator must correct the cause and explicitly request replay/recovery.

## Failure and recovery catalogue

| Failure | Durable truth | Recovery/observable behavior |
| --- | --- | --- |
| Identity unavailable after tenant commit | Tenant and provisioning operation exist; Owner milestone incomplete | Outbox retries; operation remains observable; terminal exhaustion records failure without duplicating tenant |
| SMTP temporarily unavailable | Pending Owner and activation challenge remain valid | Email retries same encrypted payload; Tenant projects delivery progress |
| Email delivery permanently fails | Tenant remains awaiting activation | Terminal delivery error is visible; authorized resend issues a new code and supersedes the prior one |
| Broker credentials rejected | Encrypted connection remains `ActionRequired` | Owner replaces credentials or disconnects; no blind automatic retry |
| Broker/network temporarily unavailable | Connection or account attempt records retryable errors | Configured retry/backoff, then safe terminal outcome |
| Some accounts fail refresh | Successful snapshots commit; failed accounts retain prior snapshots or remain unavailable | Refresh is `PartiallySucceeded`; portfolio is honest about coverage and freshness |
| All accounts fail refresh | Prior snapshots, if any, remain | Refresh is `Failed`; portfolio may be `Degraded` with stale data or `Unavailable` |
| Worker dies after broker response but before commit | No partial snapshot is visible | Message/work lease is retried; state and inbox prevent duplicate terminal effects |
| Worker commits but dies before RabbitMQ acknowledgement | State is committed and inbox records message | Redelivery is recognized and acknowledged without re-execution |
| Outbox publication delayed | Business transition remains committed with pending outbox | Publisher retries; backlog metric/alert exposes delay |
| Key provider unavailable | Encrypted credentials remain protected | Broker call is not made; safe retryable error is recorded |
| Unknown message version or corrupt payload | No business mutation | Retry policy then DLQ, audit, alert, and operation recovery |

[Back to index](index.md)
