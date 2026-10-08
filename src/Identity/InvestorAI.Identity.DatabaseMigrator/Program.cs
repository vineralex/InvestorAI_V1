using InvestorAI.Infrastructure.Configuration;
using InvestorAI.Identity.DatabaseMigrator;
using InvestorAI.Infrastructure.DatabaseMigrations;
using Microsoft.Extensions.DependencyInjection;

internal class Program
{
	private static int Main(string[] args)
	{
		try
		{
			using var services = new ServiceCollection()
				.AddIdentityMigrator()
				.BuildServiceProvider(new ServiceProviderOptions { ValidateOnBuild = true, ValidateScopes = true });
			return services.GetRequiredService<DatabaseMigrator>().Run();
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
