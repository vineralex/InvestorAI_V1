#Requires -Version 7.0
[CmdletBinding()]
param()

$ErrorActionPreference = 'Stop'
$repositoryRoot = [IO.Path]::GetFullPath((Join-Path $PSScriptRoot '../..'))
$dotenvPath = Join-Path $repositoryRoot '.env'

try {
	if (-not (Test-Path -LiteralPath $dotenvPath -PathType Leaf)) {
		throw 'Missing root .env. Use .env.example to configure the local environment.'
	}

	# Parse data only: no evaluation, expansion, or dotenv escape processing.
	$configuration = @{}
	$lineNumber = 0
	foreach ($line in [IO.File]::ReadAllLines($dotenvPath)) {
		$lineNumber++
		$entry = $line.Trim()
		if (-not $entry -or $entry.StartsWith('#')) { continue }
		if ($entry -notmatch '^([A-Za-z_][A-Za-z0-9_]*)\s*=(.*)$') {
			throw "Invalid .env entry at line $lineNumber. Expected KEY=value."
		}
		$key = $Matches[1]
		$value = $Matches[2].Trim()
		if ($value.StartsWith('"') -or $value.StartsWith("'")) {
			if ($value.Length -lt 2 -or $value[-1] -ne $value[0]) {
				throw "Unmatched .env quotes at line $lineNumber."
			}
			$value = $value.Substring(1, $value.Length - 2)
		}
		if ($value.Contains([char]0)) { throw "Invalid .env value at line $lineNumber." }
		$configuration[$key] = $value
	}

	$passwordKeys = foreach ($service in 'TENANT', 'IDENTITY', 'EMAIL', 'PORTFOLIO') {
		"${service}_MIGRATION_PASSWORD"
		"${service}_RUNTIME_PASSWORD"
	}
	foreach ($key in (@('POSTGRES_DB', 'POSTGRES_USER', 'POSTGRES_PASSWORD') + $passwordKeys)) {
		if (-not $configuration.ContainsKey($key) -or [string]::IsNullOrWhiteSpace($configuration[$key])) {
			throw "Missing required .env value: $key."
		}
	}
	$docker = (Get-Command docker -CommandType Application -ErrorAction Stop | Select-Object -First 1).Source
	$files = @(Get-ChildItem -LiteralPath (Join-Path $PSScriptRoot 'sql') -File |
		Where-Object Name -Match '^\d{3}_.+\.sql$' | Sort-Object Name)
	if ($files.Count -eq 0) { throw 'No NNN_*.sql bootstrap files found.' }

	# Compose exec only selects an existing container. An empty dotenv file and
	# placeholder interpolation values keep Compose from interpreting real secrets.
	$composeEnvironment = [IO.Path]::GetTempFileName()
	try {
		foreach ($file in $files) {
			Write-Host "Applying $($file.Name)..."
			$start = [Diagnostics.ProcessStartInfo]::new()
			$start.FileName = $docker
			$start.WorkingDirectory = $repositoryRoot
			$start.UseShellExecute = $false
			$start.RedirectStandardInput = $true
			$start.RedirectStandardOutput = $true
			$start.RedirectStandardError = $true
			$start.StandardInputEncoding = [Text.UTF8Encoding]::new($false)
			foreach ($key in 'POSTGRES_DB', 'POSTGRES_USER', 'POSTGRES_PASSWORD',
				'RABBITMQ_USER', 'RABBITMQ_PASSWORD', 'RABBITMQ_VHOST', 'PGADMIN_EMAIL', 'PGADMIN_PASSWORD') {
				$start.Environment[$key] = 'bootstrap-unused'
			}
			foreach ($key in $passwordKeys) { $start.Environment.Remove($key) | Out-Null }
			$arguments = @('compose', '--project-directory', $repositoryRoot,
				'--env-file', $composeEnvironment, '--file', (Join-Path $repositoryRoot 'compose.yaml'),
				'exec', '-T', 'postgres', 'sh', '-c',
				'IFS= read -r PGPASSWORD; export PGPASSWORD; exec psql -X -q -h 127.0.0.1 -U "$1" -d "$2" -v ON_ERROR_STOP=1 -f -',
				'bootstrap', $configuration['POSTGRES_USER'], $configuration['POSTGRES_DB'])
			foreach ($argument in $arguments) { $start.ArgumentList.Add($argument) }

			$process = [Diagnostics.Process]::new()
			$process.StartInfo = $start
			try {
				if (-not $process.Start()) { throw "Could not start Docker for $($file.Name)." }
				$stdout = $process.StandardOutput.ReadToEndAsync()
				$stderr = $process.StandardError.ReadToEndAsync()
				$payload = [Text.StringBuilder]::new()
				[void]$payload.AppendLine($configuration['POSTGRES_PASSWORD'])
				# Administrator session only: prevent statements containing credentials
				# from being logged by PostgreSQL, including on SQL failure.
				[void]$payload.AppendLine("SET log_statement = 'none'; SET log_min_error_statement = 'panic'; SET log_min_messages = 'panic';")
				foreach ($key in $passwordKeys) {
					$encoded = [Convert]::ToBase64String([Text.Encoding]::UTF8.GetBytes($configuration[$key]))
					[void]$payload.AppendLine("\set $key '$encoded'")
				}
				[void]$payload.AppendLine([IO.File]::ReadAllText($file.FullName))
				$writeFailed = $false
				try { $process.StandardInput.Write($payload.ToString()) }
				catch { $writeFailed = $true }
				finally { $process.StandardInput.Close() }
				$process.WaitForExit()
				# Never relay raw output: psql error context can contain credentials.
				[void]$stdout.GetAwaiter().GetResult()
				[void]$stderr.GetAwaiter().GetResult()
				if ($writeFailed -or $process.ExitCode -ne 0) {
					throw "Bootstrap failed in $($file.Name). Check Docker, administrator credentials and SQL. Raw diagnostics are suppressed to protect secrets."
				}
				Write-Host "Applied $($file.Name)."
			}
			finally { $process.Dispose() }
		}
	}
	finally { [IO.File]::Delete($composeEnvironment) }
	Write-Host 'PostgreSQL bootstrap completed.'
}
catch {
	Write-Error -Message $_.Exception.Message -ErrorAction Continue
	exit 1
}
