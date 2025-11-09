using System.ComponentModel.DataAnnotations;

namespace Buildify.Core.DTOs;

public class VerifyOtpDto
{
    [Required]
    [EmailAddress]
    public string Email { get; set; }
    
    [Required]
    public string OtpCode { get; set; }
}
