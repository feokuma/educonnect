using EduConnect.Application.DTOs;
using EduConnect.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace EduConnect.Controllers;

[ApiController]
[Route("auth")]
public class AuthController(IAuthService authService) : ControllerBase
{
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequestDto request, CancellationToken cancellationToken)
    {
        var response = await authService.AuthenticateAsync(request, cancellationToken);

        return response is null
            ? Unauthorized()
            : Ok(response);
    }

    [HttpPost("refresh")]
    public IActionResult Refresh([FromBody] RefreshTokenRequestDto request)
    {
        var response = authService.Refresh(request);

        return response is null
            ? Unauthorized()
            : Ok(response);
    }
}
