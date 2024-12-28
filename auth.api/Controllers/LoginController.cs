using Asp.Versioning;
using auth.api.Configuration;
using auth.request.Command.Login.v1;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace auth.api.Controllers;

[ApiVersion(1)]
[ApiController]
[Route("touch/auth/api/v{v:apiVersion}/[controller]")]
public class LoginController : CustomController
{
    private readonly ILogger<LoginController> _logger;
    private readonly IMediator _mediator;

    public LoginController(IMediator mediator, ILogger<LoginController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    [HttpPost]
    [Route("login")]
    [MapToApiVersion("1")]
    public async Task<IActionResult> Login([FromBody] LoginCommand command)
    {
        _logger.LogInformation("LoginController.Login - Start");
        var result = await _mediator.Send(command);
        return OkorBadRequestValidationApiResponse(result);
    }
}
