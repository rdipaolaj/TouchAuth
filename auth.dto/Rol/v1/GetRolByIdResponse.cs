using auth.common.Enums;
using System.Text.Json.Serialization;

namespace auth.dto.Rol.v1;
//public class GetRolByIdResponse
//{
//    public List<Rol> Roles { get; set; } = new List<Rol>();
//}

public class GetRolByIdResponse
{
    [JsonPropertyName("roleId")]
    public Guid RoleId { get; set; }

    [JsonPropertyName("roleName")]
    public string RoleName { get; set; } = string.Empty;

    [JsonPropertyName("userRoleValue")]
    public UserRole UserRoleValue { get; set; }
}