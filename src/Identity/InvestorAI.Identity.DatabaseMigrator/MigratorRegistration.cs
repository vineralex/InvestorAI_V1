using InvestorAI.Infrastructure.DatabaseMigrations;
using Microsoft.Extensions.DependencyInjection;

namespace InvestorAI.Identity.DatabaseMigrator;

public static class MigratorRegistration
{
	public static IServiceCollection AddIdentityMigrator(this IServiceCollection services) =>
		services.AddDatabaseMigrator(new MigrationDefinition(
			ServiceName: "Identity",
			Username: "investorai_identity_migration",
			PasswordEnvironmentVariable: "IDENTITY_MIGRATION_PASSWORD",
			JournalSchema: "public",
			JournalTable: "identity_schema_migrations",
			ScriptsAssembly: typeof(MigratorRegistration).Assembly,
			ScriptPrefix: "Identity.Migrations."));
}
