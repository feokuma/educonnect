using Microsoft.AspNetCore.Mvc;
using EduConnect.Application.DTOs;
using EduConnect.Application.Services;

namespace EduConnect.Controllers;

[ApiController]
[Route("users")]
public class UsersController(IUserService userService) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] CreateUserRequestDto request,
        CancellationToken cancellationToken)
    {
        var createdUser = await userService.CreateAsync(request, cancellationToken);

        return Created($"/users/{createdUser.Id}", createdUser);
    }
}
