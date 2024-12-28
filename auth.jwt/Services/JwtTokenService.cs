using auth.dto.User.v1;

namespace auth.jwt.Services;
public class JwtTokenService : IJwtTokenService
{
    private readonly IJwtTokenGenerator _tokenGenerator;

    public JwtTokenService(IJwtTokenGenerator tokenGenerator)
    {
        _tokenGenerator = tokenGenerator;
    }

    public (string Token, DateTime Expiry) GenerateToken(UserResponse user)
    {
        var token = _tokenGenerator.GenerateToken(user);
        return (token, DateTime.UtcNow.AddMinutes(10)); // Token válido por 10 minutos
    }
}