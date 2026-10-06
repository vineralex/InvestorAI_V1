using InvestorAI.DatabaseMigrations;
using Microsoft.Extensions.DependencyInjection;

namespace InvestorAI.Tenant.DatabaseMigrator;

public static class MigratorRegistration
{
	public static IServiceCollection AddTenantMigrator(this IServiceCollection services) =>
		services.AddDatabaseMigrator(new MigrationDefinition(
			ServiceName: "Tenant",
			Username: "investorai_tenant_migration",
			PasswordEnvironmentVariable: "TENANT_MIGRATION_PASSWORD",
			JournalSchema: "public",
			JournalTable: "tenant_schema_migrations",
			ScriptsAssembly: typeof(MigratorRegistration).Assembly,
			ScriptPrefix: "Tenant.Migrations."));
}
