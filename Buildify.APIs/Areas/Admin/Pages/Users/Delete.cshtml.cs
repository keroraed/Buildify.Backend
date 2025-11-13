using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Identity;
using Buildify.Core.Entities.Identity;

namespace Buildify.APIs.Areas.Admin.Pages.Users
{
    public class DeleteModel : PageModel
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly ILogger<DeleteModel> _logger;

        public DeleteModel(UserManager<AppUser> userManager, ILogger<DeleteModel> logger)
        {
            _userManager = userManager;
            _logger = logger;
        }

        [BindProperty]
        public new AppUser User { get; set; } = null!;
        public string? ErrorMessage { get; set; }

        public async Task<IActionResult> OnGetAsync(string id)
        {
            // Check if admin is authenticated
            var token = Request.Cookies["AuthToken"];
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToPage("/Account/Login", new { area = "Admin", returnUrl = $"/Admin/Users/Delete/{id}" });
            }

            if (string.IsNullOrEmpty(id))
            {
                ErrorMessage = "User ID is required.";
                return Page();
            }

            try
            {
                var user = await _userManager.FindByIdAsync(id);
                if (user == null)
                {
                    ErrorMessage = "User not found.";
                    return Page();
                }

                User = user;
            }
            catch (Exception ex)
            {
                ErrorMessage = "An error occurred while loading user.";
                _logger.LogError(ex, "Error loading user for delete. ID: {UserId}", id);
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            try
            {
                var user = await _userManager.FindByIdAsync(User.Id);
                if (user == null)
                {
                    TempData["ErrorMessage"] = "User not found.";
                    return RedirectToPage("./Index");
                }

                var result = await _userManager.DeleteAsync(user);

                if (result.Succeeded)
                {
                    TempData["SuccessMessage"] = $"User '{user.Email}' deleted successfully.";
                    return RedirectToPage("./Index");
                }
                else
                {
                    ErrorMessage = string.Join(", ", result.Errors.Select(e => e.Description));
                    User = user;
                    return Page();
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = "An error occurred while deleting the user.";
                _logger.LogError(ex, "Error deleting user. ID: {UserId}", User.Id);
                
                var user = await _userManager.FindByIdAsync(User.Id);
                if (user != null)
                {
                    User = user;
                }
                return Page();
            }
        }
    }
}
