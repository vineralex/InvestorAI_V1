namespace InvestorAI.Infrastructure.Configuration;

public sealed class DotEnvReader
{
	private readonly string baseDirectory = AppContext.BaseDirectory;

	public IReadOnlyDictionary<string, string?> ReadRepositoryFile()
	{
		var directory = new DirectoryInfo(baseDirectory);
		while (directory is not null && !File.Exists(Path.Combine(directory.FullName, "InvestorAI.slnx")))
			directory = directory.Parent;
		if (directory is null)
			throw new DotEnvReadException("Development repository root not found.");

		var values = new Dictionary<string, string?>(StringComparer.OrdinalIgnoreCase);
		var path = Path.Combine(directory.FullName, ".env");
		if (!File.Exists(path)) throw new DotEnvReadException("Development root .env file not found.");
		var lines = File.ReadAllLines(path);
		for (var index = 0; index < lines.Length; index++)
		{
			var entry = lines[index].Trim();
			if (entry.Length == 0 || entry.StartsWith('#')) continue;
			var separator = entry.IndexOf('=');
			if (separator <= 0) throw InvalidLine(index);
			var key = entry[..separator].Trim();
			if (!IsValidKey(key)) throw InvalidLine(index);
			var value = entry[(separator + 1)..].Trim();
			if (value.StartsWith('"') || value.StartsWith('\''))
			{
				if (value.Length < 2 || value[^1] != value[0]) throw InvalidLine(index);
				value = value[1..^1];
			}
			if (value.Contains('\0')) throw InvalidLine(index);
			values[key] = value;
		}
		return values;
	}

	private static bool IsValidKey(string key) => key.Length > 0
		&& (char.IsAsciiLetter(key[0]) || key[0] == '_')
		&& key.All(character => char.IsAsciiLetterOrDigit(character) || character == '_');

	private static DotEnvReadException InvalidLine(int index) =>
		new($"Invalid .env entry at line {index + 1}.");
}

public sealed class DotEnvReadException(string message) : Exception(message);
