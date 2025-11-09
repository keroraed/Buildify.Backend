using System.ComponentModel.DataAnnotations;

namespace Buildify.Core.DTOs;

public class RegisterDTO
{
    [Required]
    [EmailAddress]
    public string Email { get; set; }
    
    [Required]
    public string DisplayName { get; set; }
    
    [Required]
    [Phone]
    public string PhoneNumber { get; set; }
    
    [Required]
    public string Password { get; set; }
}
