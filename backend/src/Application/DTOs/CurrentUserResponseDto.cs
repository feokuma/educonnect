namespace EduConnect.Application.DTOs;

public record CurrentUserResponseDto(
    string Id,
    string Email,
    DateTimeOffset? TokenExpiresAt);
