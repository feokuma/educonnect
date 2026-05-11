using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using EduConnect.Application.DTOs;
using EduConnect.Application.Services;

namespace EduConnect.Controllers;

[ApiController]
[Route("users")]
public class UsersController(IUserService userService) : ControllerBase
{
    [Authorize]
    [HttpGet("me")]
    public IActionResult Me()
    {
        var subject = User.FindFirstValue("sub");
        var email = User.FindFirstValue("email");
        if (string.IsNullOrWhiteSpace(subject) || string.IsNullOrWhiteSpace(email))
        {
            return Unauthorized();
        }

        return Ok(new CurrentUserResponseDto(
            subject,
            email,
            GetTokenExpiration()));
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] CreateUserRequestDto request,
        CancellationToken cancellationToken)
    {
        var createdUser = await userService.CreateAsync(request, cancellationToken);

        return Created($"/users/{createdUser.Id}", createdUser);
    }

    private DateTimeOffset? GetTokenExpiration()
    {
        var expiresAt = User.FindFirstValue("exp");

        return long.TryParse(expiresAt, out var unixTime)
            ? DateTimeOffset.FromUnixTimeSeconds(unixTime)
            : null;
    }
}
