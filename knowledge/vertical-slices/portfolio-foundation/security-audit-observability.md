---
type: specification
title: Portfolio Foundation Security, Audit, and Observability
description: Security boundaries, encryption, tenant isolation, audit format, telemetry, and operating constraints for the Portfolio Foundation slice.
tags: [vertical-slice, security, encryption, audit, observability, multitenancy]
updated_at: 2026-09-23
status: draft
---

# Security, audit, and observability

## Deployment boundary

The first slice and the complete MVP are local-development systems. They are not
approved for production or public-internet exposure. This boundary permits
public-endpoint rate limiting to be deferred from the first slice, although
rate limiting for sign-in, activation verification, and resend remains required
before the complete MVP is declared ready.

Changing the deployment boundary requires a separate threat review and makes
internet-facing abuse protection a blocking prerequisite.

## Tenant isolation and authorization

- Every tenant-owned record carries `tenantId`.
- Every API applies permission and tenant/resource authorization independently.
- PostgreSQL Row-Level Security backs application enforcement for tenant-owned
  tables.
- Runtime roles do not own tenant tables and cannot bypass RLS.
- Tenant context is set transaction-locally so pooled connections cannot leak it
  between requests.
- Each service owns its database role and cannot query another service's tables.
- Decision models, agents, RabbitMQ payloads, request parameters, and external
  providers are never trusted sources of tenant identity.

Isolation tests that attempt cross-tenant reads and writes are readiness gates,
not optional hardening.

## Secret classes

| Secret | Owner | Persistent form | Permitted plaintext boundary |
| --- | --- | --- | --- |
| User password | Identity | ASP.NET Core Identity password hash | Password setup/sign-in request handling only |
| Activation code | Identity | Irreversible hash; encrypted delivery payload outside Identity | Identity verification and Email send memory only |
| Access/refresh token | Identity | Per OpenIddict token policy | Client and validating endpoint only; never RabbitMQ/logs |
| Broker credentials | Portfolio | Envelope-encrypted ciphertext and wrapped data key | Portfolio worker memory immediately around adapter call |
| Delivery private key | Email/key manager | External key manager; non-exportable where possible | Decrypt operation boundary only |
| Broker key-encryption key | Portfolio/key manager | External key manager in deployed environments | Key-provider operation boundary only |

## Broker envelope encryption

Portfolio Service encrypts each credential payload with a data-encryption key
and stores only ciphertext, a wrapped data key, and versioned key metadata in
PostgreSQL. The wrapping key resides outside the database and repository.

All Portfolio instances access the same logical external key manager through a
replaceable key-provider boundary. Rotation introduces a new active key version
and permits gradual rewrapping of data keys. Loss of key material is treated as
a recoverability failure; backup and recovery rules belong to the selected key
manager design.

Local development may use an explicitly marked test key from an untracked,
gitignored `.env` file that must never be committed. A deployed mode must refuse
to start with the development key provider.

## Activation delivery encryption

Identity and Email use asymmetric or equivalently permission-separated
encryption:

- Email owns the delivery-decryption key identity;
- Identity has the public key or only an `encrypt` capability;
- Identity cannot decrypt delivery payloads;
- Email has the `decrypt` capability;
- the message contains ciphertext plus key identifier/version and nonsecret
  delivery metadata;
- old key versions remain available until all messages encrypted for them have
  expired or reached terminal handling;
- integrity protection detects ciphertext modification;
- local development uses dev-only material from `.env`;
- deployed private key material remains in the external key manager where
  possible.

Exact algorithms and key-manager products require a focused security design and
are not selected by this slice specification.

## Common error model and service catalogues

Every service uses a common conceptual error envelope but owns its codes and
message catalogue. The envelope separates:

- stable namespaced code;
- safe parameters;
- retry/action classification;
- user-friendly message;
- correlation reference;
- protected diagnostics.

Long-running operations store stable codes and safe parameters, not a frozen
localized sentence. User-friendly text can therefore evolve without rewriting
operation history. There is no central Error Dictionary Service.

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
