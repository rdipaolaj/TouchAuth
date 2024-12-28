using auth.common.Settings;
using System.Text.Json.Serialization;

namespace auth.common.Secrets;
public class AuthSecrets : ISecret
{
    [JsonPropertyName("jwt-signing-key")]
    public string JwtSigningKey { get; set; } = string.Empty;
}
