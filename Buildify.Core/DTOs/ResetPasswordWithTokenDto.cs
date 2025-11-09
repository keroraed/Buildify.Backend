using System.ComponentModel.DataAnnotations;

namespace Buildify.Core.DTOs;

public class ResetPasswordWithTokenDto
{
    [Required]
    [EmailAddress]
    public string Email { get; set; }
    
    [Required]
    public string ResetToken { get; set; }
    
    [Required]
    public string NewPassword { get; set; }
}
