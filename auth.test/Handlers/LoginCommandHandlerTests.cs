using auth.common.Responses;
using auth.dto.Rol.v1;
using auth.dto.User.v1;
using auth.handler.Login;
using auth.internalservices.Rol;
using auth.internalservices.User;
using auth.jwt.Services;
using auth.request.Command.Login.v1;
using Microsoft.Extensions.Logging;
using Moq;

namespace auth.test.Handlers;

[TestFixture]
public class LoginCommandHandlerTests
{
    private Mock<ILogger<LoginCommandHandler>> _loggerMock;
    private Mock<IRoleService> _roleServiceMock;
    private Mock<IUserService> _userServiceMock;
    private Mock<IJwtTokenService> _jwtTokenServiceMock;
    private LoginCommandHandler _handler;

    [SetUp]
    public void SetUp()
    {
        _loggerMock = new Mock<ILogger<LoginCommandHandler>>();
        _roleServiceMock = new Mock<IRoleService>();
        _userServiceMock = new Mock<IUserService>();
        _jwtTokenServiceMock = new Mock<IJwtTokenService>();
        _handler = new LoginCommandHandler(
            _loggerMock.Object,
            _roleServiceMock.Object,
            _userServiceMock.Object,
            _jwtTokenServiceMock.Object
        );
    }

    [Test]
    public async Task Handle_ReturnsSuccessResponse_WhenCredentialsAreValid()
    {
        var command = new LoginCommand { Username = "testuser", Password = "password123" };
        var userResponse = new ApiResponse<UserResponse>
        {
            Success = true,
            Data = new UserResponse { Username = "testuser", UserId = Guid.NewGuid(), RoleId = Guid.NewGuid() }
        };

        var rolesResponse = new ApiResponse<List<GetRolByIdResponse>>
        {
            Success = true,
            Data = new List<GetRolByIdResponse>
            {
                new GetRolByIdResponse { RoleId = userResponse.Data.RoleId, RoleName = "Admin", UserRoleValue = auth.common.Enums.UserRole.Administrator }
            }
        };

        var jwtToken = "generatedToken";
        var tokenExpiry = DateTime.UtcNow.AddHours(1);

        _userServiceMock.Setup(s => s.GetUserAsync(It.IsAny<UserRequest>()))
                        .ReturnsAsync(userResponse);
        _roleServiceMock.Setup(s => s.GetListRolesAsync())
                        .ReturnsAsync(rolesResponse);
        _jwtTokenServiceMock.Setup(s => s.GenerateToken(It.IsAny<UserResponse>()))
                            .Returns((jwtToken, tokenExpiry));

        var result = await _handler.Handle(command, default);

        Assert.IsTrue(result.Success);
        Assert.IsNotNull(result.Data);
        Assert.AreEqual("testuser", result.Data.Username);
        Assert.AreEqual(jwtToken, result.Data.JwtToken);
    }

    [Test]
    public async Task Handle_ReturnsErrorResponse_WhenUserNotFound()
    {
        var command = new LoginCommand { Username = "testuser", Password = "password123" };
        var userResponse = new ApiResponse<UserResponse>
        {
            Success = false,
            Message = "User not found"
        };

        _userServiceMock.Setup(s => s.GetUserAsync(It.IsAny<UserRequest>()))
                        .ReturnsAsync(userResponse);

        var result = await _handler.Handle(command, default);

        Assert.IsFalse(result.Success);
        Assert.AreEqual("User not found", result.Message);
    }

    [Test]
    public async Task Handle_ReturnsErrorResponse_WhenRoleNotFound()
    {
        var command = new LoginCommand { Username = "testuser", Password = "password123" };
        var userResponse = new ApiResponse<UserResponse>
        {
            Success = true,
            Data = new UserResponse { Username = "testuser", UserId = Guid.NewGuid(), RoleId = Guid.NewGuid() }
        };

        var rolesResponse = new ApiResponse<List<GetRolByIdResponse>>
        {
            Success = true,
            Data = new List<GetRolByIdResponse>()
        };

        _userServiceMock.Setup(s => s.GetUserAsync(It.IsAny<UserRequest>()))
                        .ReturnsAsync(userResponse);
        _roleServiceMock.Setup(s => s.GetListRolesAsync())
                        .ReturnsAsync(rolesResponse);

        var result = await _handler.Handle(command, default);

        Assert.IsFalse(result.Success);
        Assert.AreEqual("El rol del usuario no es válido.", result.Message);
    }
}

