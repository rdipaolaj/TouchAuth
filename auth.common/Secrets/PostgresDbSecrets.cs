using auth.common.Settings;
using System.Text.Json.Serialization;

namespace auth.common.Secrets;
public class PostgresDbSecrets : ISecret
{
    [JsonPropertyName("username")]
    public string Username { get; set; } = string.Empty;

    [JsonPropertyName("password")]
    public string Password { get; set; } = string.Empty;

    [JsonPropertyName("host")]
    public string Host { get; set; } = string.Empty;

    [JsonPropertyName("port")]
    public string Port { get; set; } = string.Empty;

    [JsonPropertyName("dbname")]
    public string Dbname { get; set; } = string.Empty;
}