using DbUp.Engine.Output;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace InvestorAI.Tenant.DatabaseMigrator;

public static class MigratorRegistration
{
	private static readonly (string Variable, string Key)[] Aliases =
	[
		("POSTGRES_HOST", "Database:Host"),
		("POSTGRES_PORT", "Database:Port"),
		("POSTGRES_DB", "Database:Name"),
		("TENANT_MIGRATION_PASSWORD", "Database:Password")
	];

	public static IServiceCollection AddTenantMigrator(this IServiceCollection services)
	{
		services.AddSingleton<DotEnvReader>();
		services.AddSingleton<IConfiguration>(provider => BuildConfiguration(provider.GetRequiredService<DotEnvReader>()));
		services.AddOptions<MigrationSettings>()
			.BindConfiguration("Database")
			.Validate(settings => !string.IsNullOrWhiteSpace(settings.Host), "Database host is required.")
			.Validate(settings => settings.Port is >= 1 and <= 65535, "Database port must be between 1 and 65535.")
			.Validate(settings => !string.IsNullOrWhiteSpace(settings.Name), "Database name is required.")
			.Validate(settings => !string.IsNullOrWhiteSpace(settings.Password), "Migration password is required.");
		services.AddSingleton<IUpgradeLog, SafeUpgradeLog>();
		services.AddSingleton<TenantDatabaseMigrator>();
		return services;
	}

	private static IConfiguration BuildConfiguration(DotEnvReader reader)
	{
		var environment = Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT") ?? "Production";
		var development = environment.Equals("Development", StringComparison.OrdinalIgnoreCase);
		if (!development && !environment.Equals("Production", StringComparison.OrdinalIgnoreCase))
			throw new InvalidOperationException("Unknown DOTNET_ENVIRONMENT.");

		var builder = new ConfigurationBuilder()
			.SetBasePath(AppContext.BaseDirectory)
			.AddJsonFile("appsettings.json", optional: false, reloadOnChange: false);
		if (development)
			builder.AddInMemoryCollection(MapValues(reader.ReadRepositoryFile()));

		var processAliases = new Dictionary<string, string?>();
		foreach (var (variable, key) in Aliases)
		{
			var value = Environment.GetEnvironmentVariable(variable);
			if (value is not null) processAliases[key] = value;
		}
		builder.AddInMemoryCollection(processAliases).AddEnvironmentVariables();
		var configuration = builder.Build();
		if (development && string.IsNullOrWhiteSpace(configuration["Database:Host"]))
			configuration["Database:Host"] = "localhost";
		return configuration;
	}

	private static Dictionary<string, string?> MapValues(IReadOnlyDictionary<string, string?> values)
	{
		var mapped = new Dictionary<string, string?>(StringComparer.OrdinalIgnoreCase);
		foreach (var (variable, key) in Aliases)
			if (values.TryGetValue(variable, out var value)) mapped[key] = value;
		foreach (var key in new[] { "Host", "Port", "Name", "Password" })
			if (values.TryGetValue($"Database__{key}", out var value)) mapped[$"Database:{key}"] = value;
		return mapped;
	}
}
