using auth.common.Settings;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Npgsql;

namespace auth.data.HealthCheck;
public class PostgresHealthCheck : IHealthCheck
{
    private readonly PostgresDbSettings _postgresSettings;
    private readonly ILogger<PostgresHealthCheck> _logger;

    public PostgresHealthCheck(IOptions<PostgresDbSettings> postgresSettings, ILogger<PostgresHealthCheck> logger)
    {
        _postgresSettings = postgresSettings.Value;
        _logger = logger;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        if (_postgresSettings == null)
        {
            _logger.LogError("PostgresDbSettings is not configured.");
            return HealthCheckResult.Unhealthy("PostgresDbSettings is not configured.");
        }

        // Crea la cadena de conexión usando los valores configurados en _postgresSettings
        var connectionString = $"Host={_postgresSettings.Host};Port={_postgresSettings.Port};Username={_postgresSettings.Username};Password={_postgresSettings.Password};Database={_postgresSettings.Dbname};";

        try
        {
            await using var connection = new NpgsqlConnection(connectionString);
            await connection.OpenAsync(cancellationToken);
            _logger.LogInformation("Connection to PostgreSQL database was successful.");
            return HealthCheckResult.Healthy("PostgreSQL connection is healthy.");
        }
        catch (Exception ex)
        {
            _logger.LogError($"PostgreSQL connection failed: {ex.Message}");
            return HealthCheckResult.Unhealthy($"PostgreSQL connection failed: {ex.Message}");
        }
    }
}
