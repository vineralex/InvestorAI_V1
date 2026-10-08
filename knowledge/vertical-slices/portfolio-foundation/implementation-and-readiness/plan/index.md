---
type: specification
title: Implementation plan
description: Dependency order and navigation for milestones 1 through 11.
tags: [vertical-slice, implementation-order, testing, readiness, acceptance]
updated_at: 2026-10-09
status: draft
---

# Implementation plan

Milestone 1 is complete. Milestone 2 is next; later milestones remain planned.

## Dependency-ordered implementation plan

```mermaid
flowchart TD
    Z[1. Repository bootstrap]
    A[2. Contract and isolation foundation]
    B[3. Identity permissions and tokens]
    C[4. Tenant provisioning and activation]
    D[5. Email delivery]
    E[6. Portfolio connection security]
    F[7. Broker adapter and validation]
    G[8. Account selection and initial import]
    H[9. Refresh orchestration and snapshots]
    I[10. Portfolio read views]
    J[11. Recovery, observability, and end-to-end proof]

    Z --> A
    A --> B
    A --> C
    B --> C
    C --> D
    B --> E
    E --> F
    F --> G
    G --> H
    G --> I
    H --> I
    D --> J
    I --> J
    A --> J
```


- [1. Repository bootstrap](1-repository-bootstrap.md)
- [2. Contract and isolation foundation](2-contract-and-isolation-foundation.md)
- [3. Identity permissions and tokens](3-identity-permissions-and-tokens.md)
- [4. Tenant provisioning and activation](4-tenant-provisioning-and-activation.md)
- [5. Email delivery](5-email-delivery.md)
- [6. Portfolio connection security](6-portfolio-connection-security.md)
- [7. Broker adapter and validation](7-broker-adapter-and-validation.md)
- [8. Account selection and initial import](8-account-selection-and-initial-import.md)
- [9. Refresh orchestration and snapshots](9-refresh-orchestration-and-snapshots.md)
- [10. Portfolio read views](10-portfolio-read-views.md)
- [11. Recovery, observability, and end-to-end proof](11-recovery-observability-and-end-to-end-proof.md)

[Back to implementation index](../index.md)
