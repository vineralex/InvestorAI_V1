using InvestorAI.DatabaseMigrations;
using Microsoft.Extensions.DependencyInjection;

namespace InvestorAI.Email.DatabaseMigrator;

public static class MigratorRegistration
{
	public static IServiceCollection AddEmailMigrator(this IServiceCollection services) =>
		services.AddDatabaseMigrator(new MigrationDefinition(
			ServiceName: "Email",
			Username: "investorai_email_migration",
			PasswordEnvironmentVariable: "EMAIL_MIGRATION_PASSWORD",
			JournalSchema: "public",
			JournalTable: "email_schema_migrations",
			ScriptsAssembly: typeof(MigratorRegistration).Assembly,
			ScriptPrefix: "Email.Migrations."));
}
