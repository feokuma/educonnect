using Microsoft.AspNetCore.Mvc;
using EduConnect.Application.DTOs;

namespace EduConnect.Controllers;

[ApiController]
[Route("users")]
public class UsersController : ControllerBase
{
    [HttpPost]
    public IActionResult Create([FromBody] CreateUserRequestDto request)
    {
        var user = new
        {
            id = Guid.NewGuid(),
            name = request.Name,
            email = request.Email,
            createdAt = DateTimeOffset.UtcNow
        };

        return Created($"/users/{user.id}", user);
    }
}
