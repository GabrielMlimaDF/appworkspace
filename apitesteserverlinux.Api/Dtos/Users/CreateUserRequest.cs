using System.ComponentModel.DataAnnotations;

namespace apitesteserverlinux.Api.Dtos.Users;

public class CreateUserRequest
{
    [Required, MinLength(6), MaxLength(120)]
    public string Name { get; set; } = string.Empty;

    [Required, EmailAddress, MaxLength(254)]
    public string Email { get; set; } = string.Empty;

    [Required, MinLength(8), MaxLength(72)]
    public string Password { get; set; } = string.Empty;

    // Telefone no padrão E.164 (ex: +5511999999999)
    [MaxLength(20)]
    public string? PhoneNumber { get; set; }
}
