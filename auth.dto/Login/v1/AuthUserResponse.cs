using auth.common.Enums;

namespace auth.dto.Login.v1;
public class AuthUserResponse
{
    public string Username { get; set; } = string.Empty;
    public Guid UserId { get; set; }
    public UserRole UserRoleValue { get; set; }
    public string JwtToken { get; set; } = string.Empty;
    public DateTime TokenExpiry { get; set; }
}