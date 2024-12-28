using auth.dto.User.v1;

namespace auth.jwt.Services;
public interface IJwtTokenGenerator
{
    string GenerateToken(UserResponse user);
}