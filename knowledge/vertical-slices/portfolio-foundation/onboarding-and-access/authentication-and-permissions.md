---
type: specification
title: Authentication and permissions
description: Details for authentication and permissions.
tags: [vertical-slice, tenant, identity, activation, jwt, permissions]
updated_at: 2026-10-09
status: draft
---

# Authentication and permissions

## Authentication and token policy

- Identity Service uses ASP.NET Core Identity and OpenIddict.
- Access JWT lifetime is configurable and deliberately short.
- Refresh tokens are longer-lived, rotated on use, and revocable by Identity
  Service.
- Blocking a user or tenant, or changing a membership/role, prevents issuance
  and refresh of tokens immediately.
- An already issued access JWT remains valid until its short expiry; distributed
  immediate access-token revocation is not part of the slice.
- Each HTTP API validates signature, issuer, audience, lifetime, permissions,
  and tenant/resource attributes locally.
- User bearer tokens terminate at HTTP boundaries and never enter RabbitMQ.
- Async service commands use narrowly scoped internal JWTs issued by Identity
  Service for the recipient and action.

## Permissions and roles

Authorization checks effective permissions plus tenant/resource attributes. No
service should contain business authorization of the form `role == Owner`.

Conceptual permissions include:

- read portfolio and freshness;
- request portfolio refresh;
- read broker connections/accounts;
- manage broker connections and account inclusion;
- manage tenant;
- manage tenant memberships.

Exact permission names are a contract-design detail, but their meanings must be
stable and independently testable.

Roles are fixed bundles for the first slice:

| Capability | Owner | Manager | Viewer |
| --- | ---: | ---: | ---: |
| Read both portfolio views, freshness, coverage, and refresh state | Yes | Yes | Yes |
| Request manual refresh | Yes | Yes | No |
| Read safe connection/account state | Yes | As permitted by the read bundle; no secrets | As permitted by the read bundle; no secrets |
| Create, update credentials for, select accounts from, or disconnect a broker connection | Yes | No | No |
| Manage tenant lifecycle and users | Yes | No | No |

The final permission catalogue must make any read-only connection visibility
explicit; no role can ever read credential values.

There are no custom roles, direct per-user grants, or per-user denies in this
slice. Identity Service centrally defines the fixed role-to-permission mapping
and places effective permissions in JWT claims.

## Slice boundary for Manager and Viewer

The slice implements and tests Owner, Manager, and Viewer permissions, but only
the initial Owner has an end-to-end onboarding flow. Invitations and user-facing
membership creation are deferred. Manager and Viewer authorization is verified
through prepared integration-test memberships rather than a new product flow.

## Access failures

Authentication failures use neutral responses and stable service-owned error
codes. User-facing messages come from the Identity Service message catalogue;
raw Identity/OpenIddict exceptions are not exposed.

Rate limiting for login, activation verification, and resend is intentionally
deferred from this slice but remains required for the complete MVP. This is
acceptable only because the slice and MVP are local-development systems without
public-internet exposure. Any change to that deployment boundary makes public
endpoint protection a blocking prerequisite.

[Back to index](index.md)
