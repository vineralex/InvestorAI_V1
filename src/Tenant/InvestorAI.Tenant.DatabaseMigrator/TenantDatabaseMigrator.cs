using DbUp;
using DbUp.Engine;
using DbUp.Engine.Output;
using Microsoft.Extensions.Options;
using Npgsql;

namespace InvestorAI.Tenant.DatabaseMigrator;

public sealed class TenantDatabaseMigrator(IOptions<MigrationSettings> settings, IUpgradeLog upgradeLog)
{
	public int Run()
	{
		try
		{
			var connectionString = settings.Value.BuildConnectionString();
			var assembly = typeof(TenantDatabaseMigrator).Assembly;
			var names = assembly.GetManifestResourceNames()
				.Where(name => name.StartsWith("Tenant.Migrations.", StringComparison.Ordinal)
					&& name.EndsWith(".sql", StringComparison.Ordinal))
				.Order(StringComparer.Ordinal).ToArray();
			if (names.Length == 0)
			{
				Console.Error.WriteLine("No embedded Tenant migration scripts found.");
				return 1;
			}
			var scripts = names.Select(name =>
			{
				using var stream = assembly.GetManifestResourceStream(name)!;
				using var reader = new StreamReader(stream);
				return new SqlScript(name, reader.ReadToEnd());
			}).ToArray();
			var engine = DeployChanges.To.PostgresqlDatabase(connectionString)
				.WithScripts(scripts)
				.WithScriptNameComparer(StringComparer.Ordinal)
				.JournalToPostgresqlTable("public", "tenant_schema_migrations")
				.WithTransactionPerScript()
				.LogTo(upgradeLog)
				.Build();
			var result = engine.PerformUpgrade();
			if (!result.Successful)
			{
				Console.Error.WriteLine($"Migration failed: {result.ErrorScript?.Name ?? "database preparation"}.");
				ReportFailure(result.Error);
				return 1;
			}
			foreach (var script in result.Scripts)
				Console.WriteLine($"Applied {script.Name}.");
			Console.WriteLine("Tenant database migrations completed.");
			return 0;
		}
		catch (OptionsValidationException exception)
		{
			foreach (var failure in exception.Failures) Console.Error.WriteLine(failure);
			return 1;
		}
		catch (Exception exception)
		{
			ReportFailure(exception);
			return 1;
		}
	}

	private static void ReportFailure(Exception exception)
	{
		if (exception is PostgresException postgres)
			Console.Error.WriteLine($"PostgreSQL operation failed (SQLSTATE {postgres.SqlState}).");
		else
			Console.Error.WriteLine("Migration failed. Check database configuration, connectivity and SQL scripts.");
	}
}
