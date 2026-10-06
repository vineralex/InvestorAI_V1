using InvestorAI.Tenant.DatabaseMigrator;
using Microsoft.Extensions.DependencyInjection;

internal class Program
{
	private static int Main(string[] args)
	{
		try
		{
			using var services = new ServiceCollection()
				.AddTenantMigrator()
				.BuildServiceProvider(new ServiceProviderOptions { ValidateOnBuild = true, ValidateScopes = true });
			return services.GetRequiredService<TenantDatabaseMigrator>().Run();
		}
		catch (DotEnvReadException exception)
		{
			Console.Error.WriteLine(exception.Message);
			return 1;
		}
		catch
		{
			Console.Error.WriteLine("Migrator startup failed. Check environment, appsettings.json and local .env configuration.");
			return 1;
		}
	}
}