using EduConnect.Application.DTOs;

namespace EduConnect.Application.Services;

public interface IAuthService
{
    Task<AuthResponseDto?> AuthenticateAsync(LoginRequestDto request, CancellationToken cancellationToken = default);

    AuthResponseDto? Refresh(RefreshTokenRequestDto request);
}
