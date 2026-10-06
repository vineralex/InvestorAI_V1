-- Run through bootstrap.ps1. Password variables contain UTF-8 base64 values.
-- Every file owns its transaction; no infrastructure migration journal exists.
BEGIN;

SELECT format(
    'CREATE ROLE %I LOGIN NOSUPERUSER NOCREATEDB NOCREATEROLE NOREPLICATION NOBYPASSRLS PASSWORD %L',
    role_name,
    convert_from(decode(password_base64, 'base64'), 'UTF8')
)
FROM (VALUES
    ('investorai_tenant_migration', :'TENANT_MIGRATION_PASSWORD'),
    ('investorai_tenant_runtime', :'TENANT_RUNTIME_PASSWORD'),
    ('investorai_identity_migration', :'IDENTITY_MIGRATION_PASSWORD'),
    ('investorai_identity_runtime', :'IDENTITY_RUNTIME_PASSWORD'),
    ('investorai_email_migration', :'EMAIL_MIGRATION_PASSWORD'),
    ('investorai_email_runtime', :'EMAIL_RUNTIME_PASSWORD'),
    ('investorai_portfolio_migration', :'PORTFOLIO_MIGRATION_PASSWORD'),
    ('investorai_portfolio_runtime', :'PORTFOLIO_RUNTIME_PASSWORD')
) AS requested(role_name, password_base64)
WHERE NOT EXISTS (SELECT 1 FROM pg_roles WHERE rolname = requested.role_name)
\gexec

SELECT format('REVOKE CREATE, TEMPORARY ON DATABASE %I FROM PUBLIC', current_database())
\gexec

REVOKE CREATE ON SCHEMA public FROM PUBLIC;

SELECT format(
    'GRANT CONNECT ON DATABASE %I TO investorai_tenant_migration, investorai_tenant_runtime, investorai_identity_migration, investorai_identity_runtime, investorai_email_migration, investorai_email_runtime, investorai_portfolio_migration, investorai_portfolio_runtime',
    current_database()
)
\gexec

GRANT USAGE ON SCHEMA public TO
    investorai_tenant_migration, investorai_tenant_runtime,
    investorai_identity_migration, investorai_identity_runtime,
    investorai_email_migration, investorai_email_runtime,
    investorai_portfolio_migration, investorai_portfolio_runtime;

GRANT CREATE ON SCHEMA public TO
    investorai_tenant_migration, investorai_identity_migration,
    investorai_email_migration, investorai_portfolio_migration;

-- Existing roles are deliberately not inspected or repaired.
-- Service-owned DbUp migrations grant runtime access to specific objects.
COMMIT;
