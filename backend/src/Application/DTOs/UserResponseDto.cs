namespace EduConnect.Application.DTOs;

public record UserResponseDto(Guid Id, string Name, string Email, DateTimeOffset CreatedAt);
