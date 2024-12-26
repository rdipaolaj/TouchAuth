using auth.common.Enums;
using auth.common.Exceptions;
using auth.redis.Services;
using Microsoft.AspNetCore.Antiforgery;

namespace auth.api.Configuration.Security;

public class CustomAntiforgeryDataProvider(IRedisService redisService) : IAntiforgeryAdditionalDataProvider
{
    private readonly IRedisService _redisService = redisService;

    public string GetAdditionalData(HttpContext context)
    {
        string guid = Guid.NewGuid().ToString();
        string key = $"AuthService_{guid}";

        _redisService.SaveInformation(key, guid, TimeSpan.FromMinutes(1));

        return guid;
    }

    public bool ValidateAdditionalData(HttpContext context, string additionalData)
    {
        string key = $"AuthService_{additionalData}";
        string guid = _redisService.GetInformation(key);
        bool resultValidation = guid == additionalData;

        if (resultValidation)
            _redisService.DeleteInformation(key);
        else
            throw new CustomException("Error en Forgery Token", ApiErrorCode.ValidationError);

        return resultValidation;
    }
}
