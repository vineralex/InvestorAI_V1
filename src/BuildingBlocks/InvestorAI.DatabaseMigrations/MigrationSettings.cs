using Npgsql;

namespace InvestorAI.DatabaseMigrations;

public sealed class MigrationSettings
{
	public string Host { get; set; } = "";
	public int Port { get; set; } = 5432;
	public string Name { get; set; } = "";
	public string Password { get; set; } = "";

	public string BuildConnectionString(MigrationDefinition definition) => new NpgsqlConnectionStringBuilder
	{
		Host = Host,
		Port = Port,
		Database = Name,
		Username = definition.Username,
		Password = Password,
		ApplicationName = $"InvestorAI.{definition.ServiceName}.DatabaseMigrator"
	}.ConnectionString;
}
