using Npgsql;

namespace InvestorAI.Tenant.DatabaseMigrator;

public sealed class MigrationSettings
{
	public string Host { get; set; } = "";
	public int Port { get; set; } = 5432;
	public string Name { get; set; } = "";
	public string Password { get; set; } = "";

	public string BuildConnectionString() => new NpgsqlConnectionStringBuilder
	{
		Host = Host,
		Port = Port,
		Database = Name,
		Username = "investorai_tenant_migration",
		Password = Password,
		ApplicationName = "InvestorAI.Tenant.DatabaseMigrator"
	}.ConnectionString;
}
