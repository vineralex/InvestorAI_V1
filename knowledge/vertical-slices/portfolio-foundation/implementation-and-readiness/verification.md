---
type: specification
title: Verification strategy
description: Contract, database, messaging, security, and end-to-end verification strategy.
tags: [vertical-slice, implementation-order, testing, readiness, acceptance]
updated_at: 2026-10-09
status: draft
---

# Verification strategy


### Contract and unit verification

- lifecycle transitions and invalid-transition rejection;
- permission bundles and resource/tenant rules;
- error-code-to-message fallback behavior;
- refresh outcome and portfolio status derivation;
- consolidation identity/currency rules;
- retryability classification and configuration validation.

### Database integration verification

- RLS isolation with real PostgreSQL roles;
- service ownership restrictions and forbidden cross-service access;
- atomic business/audit/outbox and inbox/business/outbox transactions;
- multi-instance refresh create-or-find race;
- account snapshot atomic replacement and empty-snapshot behavior;
- worker claim expiry/recovery and operation terminalization.

### Messaging integration verification

- duplicate, delayed, out-of-order, and unsupported-version messages;
- repeated create-tenant and create-connection calls after a lost response;
- publish failure after local commit;
- consumer crash before and after commit;
- internal JWT audience/scope rejection;
- poison-message DLQ and explicit replay process;
- end-to-end correlation preservation.

### Security verification

- no secrets in logs, traces, audits, errors, messages, or database plaintext;
- envelope-encryption round trip, rotation, and unavailable-key behavior;
- activation encrypt-only/decrypt-only permission separation;
- expired, reused, superseded, and over-attempt activation codes;
- expired/wrong-audience/wrong-tenant JWTs;
- Owner/Manager/Viewer permissions and cross-tenant denial.

### End-to-end verification

At minimum, one local end-to-end run must demonstrate:

1. operator creates tenant and initial Owner;
2. activation email is delivered through the configured SMTP path;
3. Owner activates, sets a password, signs in, and obtains valid tokens;
4. Owner creates a connection and observes successful asynchronous validation;
5. Owner includes at least one real Alpaca paper account;
6. automatic initial import completes;
7. Owner reads real positions in account and consolidated views;
8. Owner or Manager launches a manual refresh and observes operation progress;
9. the resulting portfolio exposes status, coverage, currency, and freshness;
10. Viewer can read but cannot refresh or manage connections.

The decisive user outcome is step 7, not merely successful email activation.


[Back to implementation index](../index.md)
