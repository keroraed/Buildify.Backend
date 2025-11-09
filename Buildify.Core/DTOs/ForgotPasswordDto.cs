using System.ComponentModel.DataAnnotations;

namespace Buildify.Core.DTOs;

public class ForgotPasswordDto
{
    [Required]
    [EmailAddress]
    public string Email { get; set; }
}
