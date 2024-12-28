using auth.common.Responses;
using auth.common.Settings;
using auth.common.Validations;
using auth.dto.Rol.v1;
using auth.internalservices.Base;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net.Http.Json;
using System.Text.Json;

namespace auth.internalservices.Rol;
internal class RoleService : IRoleService
{
    private readonly ILogger<RoleService> _logger;
    private readonly IBaseService _baseService;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ApiSettings _settings;

    public RoleService(
        ILogger<RoleService> logger,
        IBaseService baseService,
        IHttpClientFactory httpClientFactory,
        IOptions<ApiSettings> settings)
    {
        _logger = logger;
        _baseService = baseService;
        _httpClientFactory = httpClientFactory;
        _settings = settings.Value;
    }

    public async Task<ApiResponse<List<GetRolByIdResponse>>> GetListRolesAsync()
    {
        using HttpClient httpClient = _httpClientFactory.CreateClient("CustomClient");
        string path = GetListRolesPath();

        httpClient.BaseAddress = new Uri(_settings.UrlMsUser);

        HttpResponseMessage httpResponse = await _baseService.GetAsync(httpClient, path);

        if (!CommonHttpValidation.ValidHttpResponse(httpResponse))
        {
            var errorContent = await httpResponse.Content.ReadAsStringAsync();
            _logger.LogError($"Error in GetListRolesAsync: {errorContent}");
            _logger.LogError("Respuesta HTTP inválida: {StatusCode}, Contenido: {Content}", httpResponse.StatusCode, errorContent);
            return ApiResponseHelper.CreateErrorResponse<List<GetRolByIdResponse>>("Error al consumir el servicio de roles");
        }
        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
        var response = await httpResponse.Content.ReadFromJsonAsync<ApiResponse<List<GetRolByIdResponse>>>(options);
        return response ?? ApiResponseHelper.CreateErrorResponse<List<GetRolByIdResponse>>("No se pudieron obtener los roles");
    }

    #region Private methods

    private static string GetListRolesPath()
    {
        return "touch/role/api/v1/Role/list-roles";
    }

    #endregion
}
