using InvestorAI.Infrastructure.DatabaseMigrations;
using Microsoft.Extensions.DependencyInjection;

namespace InvestorAI.Portfolio.DatabaseMigrator;

public static class MigratorRegistration
{
	public static IServiceCollection AddPortfolioMigrator(this IServiceCollection services) =>
		services.AddDatabaseMigrator(new MigrationDefinition(
			ServiceName: "Portfolio",
			Username: "investorai_portfolio_migration",
			PasswordEnvironmentVariable: "PORTFOLIO_MIGRATION_PASSWORD",
			JournalSchema: "public",
			JournalTable: "portfolio_schema_migrations",
			ScriptsAssembly: typeof(MigratorRegistration).Assembly,
			ScriptPrefix: "Portfolio.Migrations."));
}
