using System.ComponentModel.DataAnnotations;

namespace Buildify.Core.DTOs;

public class UpdateAccountProfileDto
{
    [Required]
    public string DisplayName { get; set; }
    
    [Required]
    [Phone]
    public string PhoneNumber { get; set; }
}
