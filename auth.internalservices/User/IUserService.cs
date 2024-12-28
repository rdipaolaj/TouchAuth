using auth.common.Responses;
using auth.dto.User.v1;

namespace auth.internalservices.User;

/// <summary>
/// Interfaz de ms servicio de user data
/// </summary>
public interface IUserService
{
    Task<ApiResponse<UserResponse>> GetUserAsync(UserRequest userRequest);
}
