using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Buildify.Core.Entities.Identity;

namespace Buildify.APIs.Areas.Admin.Pages.Users
{
    public class DetailsModel : PageModel
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly ILogger<DetailsModel> _logger;

        public DetailsModel(UserManager<AppUser> userManager, ILogger<DetailsModel> logger)
        {
            _userManager = userManager;
            _logger = logger;
        }

        public new AppUser? User { get; set; }
        public List<Address> Addresses { get; set; } = new();
        public int OrderCount { get; set; }
        public string? ErrorMessage { get; set; }

        public async Task<IActionResult> OnGetAsync(string id)
        {
            // Check if admin is authenticated
            var token = Request.Cookies["AuthToken"];
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToPage("/Account/Login", new { area = "Admin", returnUrl = $"/Admin/Users/Details/{id}" });
            }

            if (string.IsNullOrEmpty(id))
            {
                ErrorMessage = "User ID is required.";
                return Page();
            }

            try
            {
                // Get user with addresses
                User = await _userManager.Users
                    .Include(u => u.Addresses)
                    .FirstOrDefaultAsync(u => u.Id == id);

                if (User == null)
                {
                    ErrorMessage = "User not found.";
                    return Page();
                }

                Addresses = User.Addresses ?? new List<Address>();

                // TODO: Get order count from database when Order repository is available
                OrderCount = 0;
            }
            catch (Exception ex)
            {
                ErrorMessage = "An error occurred while loading user details.";
                _logger.LogError(ex, "Error loading user details for ID: {UserId}", id);
            }

            return Page();
        }
    }
}
