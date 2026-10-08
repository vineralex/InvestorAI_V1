---
type: specification
title: 6. Portfolio connection security
description: Scope and exit gate for this implementation milestone.
tags: [vertical-slice, implementation-order, testing, readiness, acceptance]
updated_at: 2026-10-09
status: draft
---

# 6. Portfolio connection security


- implement Portfolio ownership, RLS, permissions, local audit, and error
  catalogue;
- implement envelope-encrypted credentials and key-provider abstraction;
- support dev-only `.env` key and deployed external-provider mode boundary;
- implement create, observe, credential replacement during action-required
  state, and terminal disconnect.

Exit gate: database/message/log/trace inspection reveals no plaintext broker
credential; unavailable keys fail closed.


[Back to plan](index.md)
