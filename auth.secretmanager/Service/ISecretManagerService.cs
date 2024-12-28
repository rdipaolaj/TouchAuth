using auth.common.Secrets;

namespace auth.secretmanager.Service;
public interface ISecretManagerService
{
    Task<AuthSecrets?> GetAuthSecrets();
    Task<PostgresDbSecrets?> GetPostgresDbSecrets();
    Task<RedisSecrets?> GetRedisSecrets();
}
