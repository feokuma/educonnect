using System.ComponentModel.DataAnnotations;

namespace EduConnect.Application.DTOs;

public record ValidateTokenRequestDto(
    [Required]
    string Token);
