---
type: specification
title: 3. Identity permissions and tokens
description: Scope and exit gate for this implementation milestone.
tags: [vertical-slice, implementation-order, testing, readiness, acceptance]
updated_at: 2026-10-09
status: draft
---

# 3. Identity permissions and tokens


- implement fixed role-to-permission bundles;
- issue and locally validate short-lived access JWTs;
- rotate/revoke refresh tokens;
- prepare initial-Owner membership and activation capabilities;
- verify Owner/Manager/Viewer authorization through test memberships.

Exit gate: permission and ABAC tests cover allowed, denied, expired, wrong
tenant, wrong audience, changed membership, and refresh-token revocation cases.


[Back to plan](index.md)
