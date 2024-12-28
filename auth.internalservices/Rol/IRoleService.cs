using auth.common.Responses;
using auth.dto.Rol.v1;

namespace auth.internalservices.Rol;
public interface IRoleService
{
    Task<ApiResponse<List<GetRolByIdResponse>>> GetListRolesAsync();
}
