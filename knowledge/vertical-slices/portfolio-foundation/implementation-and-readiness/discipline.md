---
type: specification
title: Implementation discipline
description: Approved discipline for incremental implementation.
tags: [vertical-slice, implementation-order, testing, readiness, acceptance]
updated_at: 2026-10-09
status: draft
---

# Implementation discipline


Use the simplest correct implementation that preserves the approved service,
security, tenancy, and consistency boundaries. A separate project or abstraction
is introduced only when there is a second concrete implementation, a real need
to isolate a dependency, or an already approved invariant that requires a
compile-time boundary. Folder and namespace boundaries are sufficient until
one of those conditions exists.

The bootstrap decisions below do not settle the open API, message, database, or
security contracts recorded in
[Decisions and open questions](../decisions-and-open-questions/index.md).


[Back to implementation index](../index.md)
