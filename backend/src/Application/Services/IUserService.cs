using EduConnect.Application.DTOs;

namespace EduConnect.Application.Services;

public interface IUserService
{
    Task<UserResponseDto> CreateAsync(
        CreateUserRequestDto request,
        CancellationToken cancellationToken = default);
}
