# Investor AI

Investor AI is a work-in-progress investment decision-support project. The
planned MVP analyzes portfolios and watchlists against each tenant's strategy
and delivers results by email. It does not execute trades.

The current implementation provides the local infrastructure, database role
bootstrap, four service-owned migrators, and initial infrastructure tests.
Portfolio import and analysis workflows are not implemented yet.

## Getting started

See [Local development](docs/local-development.md) for prerequisites, configuration,
restore/build/test commands, container startup, database migrations, and shutdown.

## Architecture and implementation status

See the [knowledge index](knowledge/index.md) for architecture and decisions,
and [implementation and readiness](knowledge/vertical-slices/portfolio-foundation/implementation-and-readiness/index.md)
for verification evidence and outstanding milestone gates. The local setup does not establish completion of Portfolio Foundation or the product MVP.
