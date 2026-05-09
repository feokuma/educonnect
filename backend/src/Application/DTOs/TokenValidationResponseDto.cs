namespace EduConnect.Application.DTOs;

public record TokenValidationResponseDto(
    bool IsValid,
    string? Subject,
    string? Email,
    DateTimeOffset? ExpiresAt);
