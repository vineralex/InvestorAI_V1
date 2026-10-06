using DbUp.Engine.Output;

namespace InvestorAI.Tenant.DatabaseMigrator;

// DbUp messages can contain SQL or exception details. The runner reports only
// known-safe script names, outcomes and SQLSTATE instead of forwarding messages.
public sealed class SafeUpgradeLog : IUpgradeLog
{
	public void LogTrace(string format, params object[] args) { }
	public void LogDebug(string format, params object[] args) { }
	public void LogInformation(string format, params object[] args) { }
	public void LogWarning(string format, params object[] args) { }
	public void LogError(string format, params object[] args) { }
	public void LogError(Exception ex, string format, params object[] args) { }
}
