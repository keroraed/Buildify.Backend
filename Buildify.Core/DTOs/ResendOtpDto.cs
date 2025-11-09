using System.ComponentModel.DataAnnotations;

namespace Buildify.Core.DTOs;

public class ResendOtpDto
{
    [Required]
    [EmailAddress]
    public string Email { get; set; }
}
