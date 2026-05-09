using System.ComponentModel.DataAnnotations;

namespace EduConnect.Application.DTOs;

public record LoginRequestDto(
    [Required]
    [EmailAddress]
    string Email,

    [Required]
    [MinLength(6)]
    string Password);
