using EduConnect.Application.DTOs;

namespace EduConnect.Application.Services;

public interface IAuthService
{
    AuthResponseDto? Authenticate(LoginRequestDto request);

    AuthResponseDto? Refresh(RefreshTokenRequestDto request);
}
