using DbUp.Engine.Output;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace InvestorAI.DatabaseMigrations;

public static class MigratorRegistration
{
	public static IServiceCollection AddDatabaseMigrator(this IServiceCollection services, MigrationDefinition definition)
	{
		services.AddSingleton(definition);
		services.AddSingleton<DotEnvReader>();
		services.AddSingleton<MigrationConfiguration>();
		services.AddSingleton<IConfiguration>(provider => provider.GetRequiredService<MigrationConfiguration>().Build());
		services.AddOptions<MigrationSettings>()
			.BindConfiguration("Database")
			.Validate(settings => !string.IsNullOrWhiteSpace(settings.Host), "Database host is required.")
			.Validate(settings => settings.Port is >= 1 and <= 65535, "Database port must be between 1 and 65535.")
			.Validate(settings => !string.IsNullOrWhiteSpace(settings.Name), "Database name is required.")
			.Validate(settings => !string.IsNullOrWhiteSpace(settings.Password), "Migration password is required.");
		services.AddSingleton<IUpgradeLog, SafeUpgradeLog>();
		services.AddSingleton<DatabaseMigrator>();
		return services;
	}
}
