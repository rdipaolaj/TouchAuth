using auth.common.Responses;
using auth.common.Settings;
using auth.common.Validations;
using auth.dto.User.v1;
using auth.internalservices.Base;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net.Http.Json;

namespace auth.internalservices.User;
internal class UserService : IUserService
{
    private readonly ILogger<UserService> _logger;
    private readonly IBaseService _baseService;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ApiSettings _settings;

    public UserService(
        ILogger<UserService> logger,
        IBaseService baseService,
        IHttpClientFactory httpClientFactory,
        IOptions<ApiSettings> settings)
    {
        _logger = logger;
        _baseService = baseService;
        _httpClientFactory = httpClientFactory;
        _settings = settings.Value;
    }

    public async Task<ApiResponse<UserResponse>> GetUserAsync(UserRequest userRequest)
    {
        if (string.IsNullOrEmpty(userRequest.Username))
            return ApiResponseHelper.CreateErrorResponse<UserResponse>("Invalid username");

        if (string.IsNullOrEmpty(userRequest.Password))
            return ApiResponseHelper.CreateErrorResponse<UserResponse>("Invalid password");

        using HttpClient httpClient = _httpClientFactory.CreateClient("CustomClient");
        string path = GetUserPath();
        httpClient.BaseAddress = new Uri(_settings.UrlMsUser);
        HttpResponseMessage httpResponse;
        try
        {
            httpResponse = await _baseService.PostAsJsonAsync(httpClient, path, userRequest);
        }
        catch (Exception ex)
        {
            _logger.LogError("Error in GetUserAsync: {ExceptionMessage}", ex.Message);
            return ApiResponseHelper.CreateErrorResponse<UserResponse>("Error connecting to user service.");
        }

        if (!CommonHttpValidation.ValidHttpResponse(httpResponse))
        {
            var errorContent = await httpResponse.Content.ReadAsStringAsync();

            try
            {
                // Intentar deserializar el error para obtener más detalles
                var errorResponse = await httpResponse.Content.ReadFromJsonAsync<ApiResponse<UserResponse>>();

                if (errorResponse != null)
                {
                    _logger.LogError("Error in GetUserAsync: {ErrorMessage}", errorResponse.Message);
                    return ApiResponseHelper.CreateErrorResponse<UserResponse>(errorResponse.Message, errorResponse.StatusCode);
                }
            }
            catch
            {
                // Si no es posible deserializar, loguear el contenido del error
                _logger.LogError("Error in GetUserAsync: {ErrorContent}", errorContent);
            }

            return ApiResponseHelper.CreateErrorResponse<UserResponse>(
                $"Invalid HTTP response: {httpResponse.StatusCode}",
                (int)httpResponse.StatusCode);
        }

        var response = await httpResponse.Content.ReadFromJsonAsync<ApiResponse<UserResponse>>();
        if (response == null || response.Data == null || !response.Success)
        {
            var errorMessage = response?.Message ?? "No se pudo obtener el usuario.";
            _logger.LogError("No se pudo obtener el usuario: {ErrorMessage}", errorMessage);
            return ApiResponseHelper.CreateErrorResponse<UserResponse>(errorMessage, response?.StatusCode ?? 500);
        }

        _logger.LogInformation("La carga del usuario fue exitosa");
        return response;
    }

    #region Private methods

    private static string GetUserPath()
    {
        return $"touch/user/api/v1/User/get-user";
    }

    #endregion
}
