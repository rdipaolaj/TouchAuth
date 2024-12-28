using auth.dto.User.v1;

namespace auth.jwt.Services;
public interface IJwtTokenService
{
    (string Token, DateTime Expiry) GenerateToken(UserResponse user);
}