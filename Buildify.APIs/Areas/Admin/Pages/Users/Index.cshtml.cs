using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Identity;
using Buildify.Core.Entities.Identity;

namespace Buildify.APIs.Areas.Admin.Pages.Users
{
    public class IndexModel : PageModel
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly ILogger<IndexModel> _logger;

        public IndexModel(UserManager<AppUser> userManager, ILogger<IndexModel> logger)
        {
            _userManager = userManager;
            _logger = logger;
        }

        public List<AppUser> Users { get; set; } = new();
        public string? ErrorMessage { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            // Check if admin is authenticated
            var token = Request.Cookies["AuthToken"];
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToPage("/Account/Login", new { area = "Admin", returnUrl = "/Admin/Users" });
            }

            try
            {
                // Get all users from UserManager
                Users = _userManager.Users
                    .OrderByDescending(u => u.DateCreated)
                    .ToList();
            }
            catch (Exception ex)
            {
                ErrorMessage = "An error occurred while loading users.";
                _logger.LogError(ex, "Error loading users");
            }

            return Page();
        }
    }
}
