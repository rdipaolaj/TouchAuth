using Amazon;
using Amazon.Runtime;
using Amazon.SecretsManager;
using Amazon.SecretsManager.Model;
using auth.common.Secrets;
using auth.common.Settings;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Diagnostics;
using System.Text.Json;

namespace auth.secretmanager.Service;
internal class SecretManagerService : ISecretManagerService
{
    private readonly IOptions<SecretManagerSettings> _settings;
    private readonly AmazonSecretsManagerClient _client;
    private readonly ILogger<SecretManagerService> _logger;

    public SecretManagerService(IOptions<SecretManagerSettings> settings, ILogger<SecretManagerService> logger)
    {
        _settings = settings;
        _logger = logger;
        if (_settings.Value.UseLocalstack)
        {
            _client = new AmazonSecretsManagerClient(
                new BasicAWSCredentials(_settings.Value.AWSAccessKey, _settings.Value.AWSSecretKey),
                new AmazonSecretsManagerConfig
                {
                    ServiceURL = _settings.Value.ServiceURL // URL de LocalStack
                });
            _logger.LogInformation("Usando LocalStack para AWS Secrets Manager");
        }
        else
        {
            _client = new AmazonSecretsManagerClient(RegionEndpoint.GetBySystemName(_settings.Value.Region));
            _logger.LogInformation("Usando AWS Secrets Manager en el entorno real");
        }
    }

    public async Task<AuthSecrets?> GetAuthSecrets()
        => await GetSecret<AuthSecrets>(_settings.Value.ArnAuthSecrets);

    public async Task<PostgresDbSecrets?> GetPostgresDbSecrets()
        => await GetSecret<PostgresDbSecrets>(_settings.Value.ArnPostgresSecrets);

    public async Task<RedisSecrets?> GetRedisSecrets()
        => await GetSecret<RedisSecrets>(_settings.Value.ArnRedisSecrets);

    private async Task<T?> GetSecret<T>(string arn) where T : ISecret
    {
        T? result = default;
        Stopwatch stopwatch = new();
        stopwatch.Start();

        _logger.LogInformation("Obteniendo valores de secret manager con Arn {arn}", arn);

        try
        {
            GetSecretValueResponse response = await _client.GetSecretValueAsync(new GetSecretValueRequest { SecretId = arn });
            result = JsonSerializer.Deserialize<T>(response.SecretString);
            stopwatch.Stop();

            _logger.LogInformation("Valores obtenidos de Arn {arn} satisfactorios, Duración ms : {ElapsedMilliseconds}",
                arn, stopwatch.ElapsedMilliseconds);
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            _logger.LogError("Error al obtener valores de secret manager con Arn {arn}, Duración ms : {ElapsedMilliseconds}, Error : {Message}",
                arn, stopwatch.ElapsedMilliseconds, ex.Message);
        }

        return result;
    }
}