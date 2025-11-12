using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Buildify.Core.DTOs;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace Buildify.APIs.Areas.Admin.Pages.Categories
{
    public class CreateModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<CreateModel> _logger;

        public CreateModel(IHttpClientFactory httpClientFactory, ILogger<CreateModel> logger)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
        }

        [BindProperty]
        public CreateCategoryDto Category { get; set; } = new();
        
        public string? ErrorMessage { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            var token = Request.Cookies["AuthToken"];
            
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToPage("/Account/Login", new { area = "Admin", returnUrl = "/Admin/Categories/Create" });
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var token = Request.Cookies["AuthToken"];
            
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToPage("/Account/Login", new { area = "Admin", returnUrl = "/Admin/Categories/Create" });
            }

            if (!ModelState.IsValid)
            {
                return Page();
            }

            try
            {
                var httpClient = _httpClientFactory.CreateClient();
                var baseUrl = $"{Request.Scheme}://{Request.Host}";
                
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var json = JsonSerializer.Serialize(Category);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await httpClient.PostAsync($"{baseUrl}/api/categories", content);

                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = "Category created successfully!";
                    return RedirectToPage("./Index");
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    _logger.LogWarning("Unauthorized access - token invalid or expired");
                    Response.Cookies.Delete("AuthToken");
                    return RedirectToPage("/Account/Login", new { area = "Admin", returnUrl = "/Admin/Categories/Create" });
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    ErrorMessage = "A category with this name already exists.";
                    _logger.LogError("Failed to create category. Error: {Error}", errorContent);
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    ErrorMessage = $"Failed to create category. Status: {response.StatusCode}";
                    _logger.LogError("Failed to create category. Status: {StatusCode}, Error: {Error}", response.StatusCode, errorContent);
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = "An error occurred while creating the category.";
                _logger.LogError(ex, "Error creating category");
            }

            return Page();
        }
    }
}
