using System.Reflection;

namespace InvestorAI.Infrastructure.DatabaseMigrations;

// Service-owned metadata. No credentials or SQL scripts live in this library.
public sealed record MigrationDefinition(
	string ServiceName,
	string Username,
	string PasswordEnvironmentVariable,
	string JournalSchema,
	string JournalTable,
	Assembly ScriptsAssembly,
	string ScriptPrefix);
