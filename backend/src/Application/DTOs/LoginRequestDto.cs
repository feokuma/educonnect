using System.ComponentModel.DataAnnotations;

namespace EduConnect.Application.DTOs;

public record LoginRequestDto(
    [Required]
    string Identifier,

    [Required]
    [MinLength(6)]
    string Password);
