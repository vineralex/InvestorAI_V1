using Microsoft.Extensions.Configuration;

namespace InvestorAI.DatabaseMigrations;

public sealed class MigrationConfiguration(DotEnvReader reader, MigrationDefinition definition)
{
	private readonly (string Variable, string Key)[] aliases =
	[
		("POSTGRES_HOST", "Database:Host"),
		("POSTGRES_PORT", "Database:Port"),
		("POSTGRES_DB", "Database:Name"),
		(definition.PasswordEnvironmentVariable, "Database:Password")
	];

	public IConfiguration Build()
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
		foreach (var (variable, key) in aliases)
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

	private Dictionary<string, string?> MapValues(IReadOnlyDictionary<string, string?> values)
	{
		var mapped = new Dictionary<string, string?>(StringComparer.OrdinalIgnoreCase);
		foreach (var (variable, key) in aliases)
			if (values.TryGetValue(variable, out var value)) mapped[key] = value;
		foreach (var key in new[] { "Host", "Port", "Name", "Password" })
			if (values.TryGetValue($"Database__{key}", out var value)) mapped[$"Database:{key}"] = value;
		return mapped;
	}
}
