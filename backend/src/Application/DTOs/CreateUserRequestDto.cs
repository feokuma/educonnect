namespace EduConnect.Application.DTOs;

public record CreateUserRequestDto(
    string Name,
    string Email,
    string Username,
    string PasswordHash);
