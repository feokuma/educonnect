using System.ComponentModel.DataAnnotations;

namespace EduConnect.Application.DTOs;

public record RefreshTokenRequestDto(
    [Required]
    string RefreshToken);
