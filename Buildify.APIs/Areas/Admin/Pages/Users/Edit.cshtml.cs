using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Identity;
using Buildify.Core.Entities.Identity;

namespace Buildify.APIs.Areas.Admin.Pages.Users
{
    public class EditModel : PageModel
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly ILogger<EditModel> _logger;

        public EditModel(UserManager<AppUser> userManager, ILogger<EditModel> logger)
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
                return RedirectToPage("/Account/Login", new { area = "Admin", returnUrl = $"/Admin/Users/Edit/{id}" });
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
                _logger.LogError(ex, "Error loading user for edit. ID: {UserId}", id);
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            try
            {
                var existingUser = await _userManager.FindByIdAsync(User.Id);
                if (existingUser == null)
                {
                    TempData["ErrorMessage"] = "User not found.";
                    return RedirectToPage("./Index");
                }

                // Update user properties
                existingUser.DisplayName = User.DisplayName;
                existingUser.Email = User.Email;
                existingUser.UserName = User.UserName;
                existingUser.PhoneNumber = User.PhoneNumber;
                existingUser.EmailConfirmed = User.EmailConfirmed;
                existingUser.PhoneNumberConfirmed = User.PhoneNumberConfirmed;

                var result = await _userManager.UpdateAsync(existingUser);

                if (result.Succeeded)
                {
                    TempData["SuccessMessage"] = $"User '{existingUser.Email}' updated successfully.";
                    return RedirectToPage("./Index");
                }
                else
                {
                    ErrorMessage = string.Join(", ", result.Errors.Select(e => e.Description));
                    return Page();
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = "An error occurred while updating the user.";
                _logger.LogError(ex, "Error updating user. ID: {UserId}", User.Id);
                return Page();
            }
        }
    }
}
