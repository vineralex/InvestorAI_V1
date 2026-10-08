---
type: specification
title: 5. Email delivery
description: Scope and exit gate for this implementation milestone.
tags: [vertical-slice, implementation-order, testing, readiness, acceptance]
updated_at: 2026-10-09
status: draft
---

# 5. Email delivery


- implement encrypted activation payload contract;
- implement Email-owned composition, SMTP delivery state, retries, and terminal
  facts;
- verify resend supersedes prior activation code;
- verify delivery failure does not roll back tenant or pending Owner.

Exit gate: activation email can be sent through the configured local/test SMTP
path without exposing the code in logs, audit, or stored plaintext history.


[Back to plan](index.md)
