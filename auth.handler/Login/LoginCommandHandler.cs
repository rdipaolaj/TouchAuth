using auth.common.Responses;
using auth.dto.Login.v1;
using auth.dto.User.v1;
using auth.internalservices.Rol;
using auth.internalservices.User;
using auth.jwt.Services;
using auth.request.Command.Login.v1;
using MediatR;
using Microsoft.Extensions.Logging;

namespace auth.handler.Login;
public class LoginCommandHandler : IRequestHandler<LoginCommand, ApiResponse<AuthUserResponse>>
{
    private readonly ILogger<LoginCommandHandler> _logger;
    private readonly IRoleService _roleService;
    private readonly IUserService _userService;
    private readonly IJwtTokenService _jwtTokenService;

    public LoginCommandHandler(
        ILogger<LoginCommandHandler> logger,
        IRoleService roleService,
        IUserService userService,
        IJwtTokenService jwtTokenService)
    {
        _logger = logger;
        _roleService = roleService;
        _userService = userService;
        _jwtTokenService = jwtTokenService;
    }

    public async Task<ApiResponse<AuthUserResponse>> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("LoginCommandHandler.Handle - Start");

        var response = new ApiResponse<AuthUserResponse>();
        var requestUser = new UserRequest
        {
            Username = request.Username,
            Password = request.Password
        };

        var userTask = _userService.GetUserAsync(requestUser);
        var rolesTask = _roleService.GetListRolesAsync();

        await Task.WhenAll(userTask, rolesTask);

        var userResponse = await userTask;
        var rolesResponse = await rolesTask;

        if (userResponse == null || userResponse.Data == null || !userResponse.Success)
        {
            _logger.LogError("Error al obtener el usuario: {Message}", userResponse?.Message ?? "Unknown error");
            return ApiResponseHelper.CreateErrorResponse<AuthUserResponse>(
                userResponse?.Message ?? "Error al obtener el usuario",
                userResponse?.StatusCode ?? 500
            );
        }

        if (rolesResponse == null || rolesResponse.Data == null || !rolesResponse.Success)
        {
            _logger.LogError("Error al obtener los roles: {Message}", rolesResponse?.Message ?? "Unknown error");
            return ApiResponseHelper.CreateErrorResponse<AuthUserResponse>(
                rolesResponse?.Message ?? "Error al obtener los roles",
                rolesResponse?.StatusCode ?? 500
            );
        }

        var userRole = rolesResponse.Data.FirstOrDefault(r => r.RoleId == userResponse.Data.RoleId);
        if (userRole == null)
        {
            _logger.LogError("El RoleId del usuario no coincide con ningún rol disponible.");
            return ApiResponseHelper.CreateErrorResponse<AuthUserResponse>(
                "El rol del usuario no es válido.",
                400
            );
        }

        var (jwtToken, tokenExpiry) = _jwtTokenService.GenerateToken(userResponse.Data);

        response.Success = true;
        response.Data = new AuthUserResponse
        {
            Username = userResponse.Data.Username,
            UserId = userResponse.Data.UserId,
            UserRoleValue = userRole.UserRoleValue,
            JwtToken = jwtToken,
            TokenExpiry = tokenExpiry
        };
        response.Message = "Login exitoso";

        _logger.LogInformation("LoginCommandHandler.Handle - Success");
        return response;
    }
}
