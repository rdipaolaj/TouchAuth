using auth.common.Responses;
using auth.dto.Login.v1;
using MediatR;

namespace auth.request.Command.Login.v1;
public class LoginCommand : IRequest<ApiResponse<AuthUserResponse>>
{
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}
