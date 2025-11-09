using Microsoft.AspNetCore.Identity;

namespace Buildify.Core.Entities.Identity;

public class AppUser : IdentityUser
{
    public string DisplayName { get; set; }
    public List<Address> Addresses { get; set; } = new List<Address>();
    public DateTime DateCreated { get; set; } = DateTime.UtcNow;
    public DateTime? LastLoginDate { get; set; }
    public string? FcmDeviceToken { get; set; }
}
