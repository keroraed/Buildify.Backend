using System.ComponentModel.DataAnnotations;

namespace Buildify.Core.DTOs;

public class LoginDTO
{
    [Required]
    [Phone]
    public string PhoneNumber { get; set; }
    
    [Required]
    public string Password { get; set; }
}
