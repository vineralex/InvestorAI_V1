---
type: specification
title: Audit and observability
description: Details for audit and observability.
tags: [vertical-slice, security, encryption, audit, observability, multitenancy]
updated_at: 2026-10-09
status: draft
---

# Audit and observability

## Local audit

Each service appends audit records to service-owned tables using its own
migrations, repository, transaction, and runtime role. There is no shared audit
table and no service reads another service's audit data.

A small versioned common library may define the audit envelope, validation, and
redaction rules. It must not contain shared SQL, migrations, repositories, or
service-specific action enums.

Minimum audit envelope:

- globally unique `auditEventId`;
- `schemaVersion` and source service;
- tenant and safe actor identity;
- action and target resource type/identifier;
- occurrence and recording times;
- outcome and stable error codes;
- operation and correlation identifiers;
- safe structured details.

Required audited actions include tenant provisioning transitions, activation
issuance/resend/success/blocking, token-security events appropriate for audit,
connection creation/credential replacement/disconnection, account inclusion,
manual refresh requests/reuse/results, and recovery/operator actions.

Audit records never contain passwords, tokens, activation codes, broker
credentials, encryption keys, raw provider payloads, or unsafe exception text.
Retention is configurable.

### Future hybrid audit path

The local envelope is deliberately shaped as a future `AuditRecorded` event.
Later, services may publish local records through outbox to a centralized
search/reporting projection, deduplicated by `auditEventId` and backfilled from
local stores. Local records remain authoritative; an exclusively centralized
synchronous audit is not the intended migration path. No Audit Service or audit
event publication is implemented in this slice.

## Structured logs and traces

- Logs are structured and describe state transitions, identifiers, duration,
  outcomes, retry counts, and stable errors.
- Distributed trace context propagates through HTTP, outbox publication,
  RabbitMQ consumption, broker calls, and SMTP calls.
- Where semantically and safely available, telemetry carries service,
  `tenantId`, `operationId`, `correlationId`, `causationId`, and `messageId`.
- Sensitive values, full financial payloads, and raw third-party responses are
  redacted or omitted.
- Instrumentation is compatible with OpenTelemetry, but the telemetry backend is
  not selected here.

## Minimum metrics

- provisioning operations by state/outcome/duration;
- activation delivery attempts, terminal failures, and latency;
- authentication and activation outcomes without identity enumeration;
- connection validations by outcome/error/duration;
- initial imports and refreshes by state, duration, target count, and outcome;
- per-account retry/failure counts;
- counts of fresh, stale, unavailable, included, and represented accounts;
- outbox and inbox backlog/age;
- dead-letter count and age;
- stuck/recovered operation count;
- key-provider, RabbitMQ, PostgreSQL, SMTP, and broker dependency failures.

Metric dimensions must avoid unbounded user/account/message identifiers where
they would create high cardinality. Detailed identifiers belong in traces and
audit, not metric labels.

## Health and alerting

Liveness proves process health without depending on optional external services.
Readiness covers the local database, RabbitMQ, key provider, and other components
required for that process to accept its work safely. Broker and SMTP outages are
normally dependency-state metrics rather than reasons to restart every service.

Configurable alerts cover stuck provisioning/refresh operations, repeated email
or broker failure, stale/unavailable portfolio coverage, old outbox/inbox
backlogs, dead-letter growth, RLS/security failures, and key-provider failure.

[Back to index](index.md)
