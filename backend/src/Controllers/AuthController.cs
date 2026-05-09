using EduConnect.Application.DTOs;
using EduConnect.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace EduConnect.Controllers;

[ApiController]
[Route("auth")]
public class AuthController(IAuthService authService) : ControllerBase
{
    [HttpPost("login")]
    public IActionResult Login([FromBody] LoginRequestDto request)
    {
        var response = authService.Authenticate(request);

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

    [HttpPost("validate")]
    public IActionResult Validate([FromBody] ValidateTokenRequestDto request)
    {
        var response = authService.Validate(request);

        return response.IsValid
            ? Ok(response)
            : Unauthorized(response);
    }
}
