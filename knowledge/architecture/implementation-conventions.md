---
type: policy
title: Solution Implementation Conventions
description: Approved dependency injection and object creation rules across the Investor AI solution.
tags: [implementation, dependency-injection, conventions]
updated_at: 2026-10-06
status: approved
---

# Dependency injection

Application code does not create services or their dependencies with `new`.
The DI container creates them; dependencies are passed through constructors.
This rule applies across the entire solution, not only to `Program.cs` or the
Tenant database migrator.

DTOs, value objects, and technical objects such as
`NpgsqlConnectionStringBuilder`, streams, configuration builders, and DbUp
script descriptors are outside this restriction. DI containers and framework
builders may construct objects as part of their own composition mechanisms.

The rule does not require a Generic Host or a shared dependency-injection
framework. The Tenant one-shot migrator uses `ServiceCollection`; service
executables may use the DI container provided by their existing host.

Follow the [implementation discipline](../vertical-slices/portfolio-foundation/implementation-and-readiness.md)
when deciding whether an abstraction or another project is warranted.
