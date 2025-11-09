using Buildify.Core.Entities.Identity;
using Microsoft.AspNetCore.Identity;

namespace Buildify.Repository.Identity;

public class AppIdentityDbContextSeed
{
    public static async Task SeedUsersAsync(UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager)
    {
        // Seed Roles
        if (!await roleManager.RoleExistsAsync("Admin"))
        {
            await roleManager.CreateAsync(new IdentityRole("Admin"));
        }

        if (!await roleManager.RoleExistsAsync("User"))
        {
            await roleManager.CreateAsync(new IdentityRole("User"));
        }

        // Seed Admin User
        if (await userManager.FindByEmailAsync("admin@example.com") == null)
        {
            var adminUser = new AppUser
            {
                DisplayName = "Administrator",
                Email = "admin@example.com",
                UserName = "admin@example.com",
                PhoneNumber = "1234567890",
                DateCreated = DateTime.UtcNow
            };

            var result = await userManager.CreateAsync(adminUser, "Admin@123");

            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(adminUser, "Admin");
            }
        }
    }
}
