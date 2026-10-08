using InvestorAI.Infrastructure.DatabaseMigrations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Npgsql;
using Xunit;

namespace InvestorAI.Infrastructure.Tests;

public sealed class MigrationSettingsTests
{
	private static MigrationDefinition Definition => new(
		"Tenant", "investorai_tenant_migration", "TENANT_MIGRATION_PASSWORD",
		"public", "tenant_schema_migrations", typeof(MigrationSettingsTests).Assembly, "Tenant.Migrations.");

	[Theory]
	[InlineData("Host", "", "Database host is required.")]
	[InlineData("Host", " ", "Database host is required.")]
	[InlineData("Port", "0", "Database port must be between 1 and 65535.")]
	[InlineData("Port", "65536", "Database port must be between 1 and 65535.")]
	[InlineData("Name", "", "Database name is required.")]
	[InlineData("Password", "", "Migration password is required.")]
	[InlineData("Password", " ", "Migration password is required.")]
	public void InvalidSettingsFailBeforeDatabaseExecution(string key, string value, string expectedError)
	{
		using var provider = CreateProvider(key, value);
		var exception = Assert.Throws<OptionsValidationException>(() =>
			provider.GetRequiredService<IOptions<MigrationSettings>>().Value);
		Assert.Contains(expectedError, exception.Failures);
	}

	[Theory]
	[InlineData(1)]
	[InlineData(5432)]
	[InlineData(65535)]
	public void ValidPortBoundariesAreAccepted(int port)
	{
		using var provider = CreateProvider("Port", port.ToString(System.Globalization.CultureInfo.InvariantCulture));
		Assert.Equal(port, provider.GetRequiredService<IOptions<MigrationSettings>>().Value.Port);
	}

	[Fact]
	public void ConnectionStringPreservesLiteralPasswordAndServiceIdentity()
	{
		const string password = "quotes\"'; backslash\\ dollar$ backtick` hash# equals= unicode\u00E9\u03A9\u4E2D";
		var settings = new MigrationSettings
		{
			Host = "localhost", Port = 5432, Name = "investorai", Password = password
		};
		var connection = new NpgsqlConnectionStringBuilder(settings.BuildConnectionString(Definition));
		Assert.Equal(password, connection.Password);
		Assert.Equal("investorai_tenant_migration", connection.Username);
		Assert.Equal("InvestorAI.Tenant.DatabaseMigrator", connection.ApplicationName);
		Assert.Equal("localhost", connection.Host);
		Assert.Equal(5432, connection.Port);
		Assert.Equal("investorai", connection.Database);
	}

	private static ServiceProvider CreateProvider(string key, string value)
	{
		var values = new Dictionary<string, string?>
		{
			["Database:Host"] = "localhost",
			["Database:Port"] = "5432",
			["Database:Name"] = "investorai",
			["Database:Password"] = "synthetic-test-password"
		};
		values[$"Database:{key}"] = value;
		var services = new ServiceCollection();
		services.AddDatabaseMigrator(Definition);
		services.AddSingleton<IConfiguration>(new ConfigurationBuilder().AddInMemoryCollection(values).Build());
		return services.BuildServiceProvider();
	}
}
