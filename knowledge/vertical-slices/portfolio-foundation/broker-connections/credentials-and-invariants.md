---
type: specification
title: Broker credentials and invariants
description: Details for broker credentials and invariants.
tags: [vertical-slice, portfolio, broker, alpaca, credentials, accounts]
updated_at: 2026-10-09
status: draft
---

# Broker credentials and invariants

## Credential handling

Broker credentials are owned only by Portfolio Service and use envelope
encryption:

1. credentials are encrypted with a data-encryption key;
2. that data key is wrapped by a versioned key-encryption key;
3. PostgreSQL stores ciphertext, wrapped data key, and key metadata, never the
   unencrypted key-encryption key;
4. messages carry `connectionId`, not credentials;
5. the worker retrieves and decrypts credentials immediately before an adapter
   call;
6. plaintext exists only in memory for the shortest practical duration;
7. key unavailability prevents the broker call and produces safe stable errors;
8. rotation supports new active key versions and gradual rewrapping.

Local development uses a test key supplied through `.env`. A deployed environment
must use an external centralized key manager through a replaceable key-provider
boundary. The specific KMS/Vault product and cryptographic algorithms remain
open infrastructure and low-level security decisions.

## Errors and user-facing messages

Portfolio Service owns stable namespaced connection and adapter error codes.
An error separates:

- stable code;
- safe structured parameters;
- retryability/action requirement;
- user-friendly text resolved through the service message catalogue;
- protected diagnostics that are never returned to the caller.

The catalogue is a service-local component following a common error envelope,
not a central Error Dictionary Service. Full localization and administrative
message editing are deferred, but the separation is required in the slice.

## Broker connection invariants

- Credentials never appear in a URL, message, log, trace attribute, audit detail,
  exception response, or snapshot.
- Validation success is not account inclusion.
- Account inclusion is not proof that a usable snapshot exists.
- A connection operation is idempotent under message redelivery.
- Duplicate external accounts must not silently create duplicate included
  sources; the precise provider-identity constraint is deferred to contract
  design.
- Portfolio Service never invokes a broker write/trading operation.

[Back to index](index.md)
