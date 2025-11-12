using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Buildify.Core.DTOs;
using System.Net.Http.Headers;
using System.Text.Json;

namespace Buildify.APIs.Areas.Admin.Pages.Categories
{
    public class DeleteModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<DeleteModel> _logger;

        public DeleteModel(IHttpClientFactory httpClientFactory, ILogger<DeleteModel> logger)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
        }

        [BindProperty(SupportsGet = true)]
        public int Id { get; set; }
        
        public CategoryDto? Category { get; set; }
        public int ProductCount { get; set; }
        public string? ErrorMessage { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            var token = Request.Cookies["AuthToken"];
            
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToPage("/Account/Login", new { area = "Admin", returnUrl = $"/Admin/Categories/Delete/{Id}" });
            }

            try
            {
                var httpClient = _httpClientFactory.CreateClient();
                var baseUrl = $"{Request.Scheme}://{Request.Host}";
                
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var response = await httpClient.GetAsync($"{baseUrl}/api/categories/{Id}");

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    };
                    Category = JsonSerializer.Deserialize<CategoryDto>(content, options);
                    
                    // Check how many products are in this category
                    await CheckProductCount(httpClient, baseUrl);
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    _logger.LogWarning("Unauthorized access - token invalid or expired");
                    Response.Cookies.Delete("AuthToken");
                    return RedirectToPage("/Account/Login", new { area = "Admin", returnUrl = $"/Admin/Categories/Delete/{Id}" });
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    TempData["ErrorMessage"] = "Category not found.";
                    return RedirectToPage("./Index");
                }
                else
                {
                    ErrorMessage = $"Failed to load category. Status: {response.StatusCode}";
                    _logger.LogError("Failed to load category. Status code: {StatusCode}", response.StatusCode);
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = "An error occurred while loading the category.";
                _logger.LogError(ex, "Error loading category");
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var token = Request.Cookies["AuthToken"];
            
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToPage("/Account/Login", new { area = "Admin", returnUrl = $"/Admin/Categories/Delete/{Id}" });
            }

            try
            {
                var httpClient = _httpClientFactory.CreateClient();
                var baseUrl = $"{Request.Scheme}://{Request.Host}";
                
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var response = await httpClient.DeleteAsync($"{baseUrl}/api/categories/{Id}");

                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = "Category deleted successfully!";
                    return RedirectToPage("./Index");
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    _logger.LogWarning("Unauthorized access - token invalid or expired");
                    Response.Cookies.Delete("AuthToken");
                    return RedirectToPage("/Account/Login", new { area = "Admin", returnUrl = $"/Admin/Categories/Delete/{Id}" });
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    TempData["ErrorMessage"] = "Category not found.";
                    return RedirectToPage("./Index");
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
                {
                    ErrorMessage = "Cannot delete category with existing products. Please remove or reassign all products first.";
                    _logger.LogError("Failed to delete category - has products");
                    
                    // Reload category data
                    await OnGetAsync();
                    return Page();
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    ErrorMessage = $"Failed to delete category. Status: {response.StatusCode}";
                    _logger.LogError("Failed to delete category. Status: {StatusCode}, Error: {Error}", response.StatusCode, errorContent);
                    
                    // Reload category data
                    await OnGetAsync();
                    return Page();
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = "An error occurred while deleting the category.";
                _logger.LogError(ex, "Error deleting category");
                
                // Reload category data
                await OnGetAsync();
                return Page();
            }
        }

        private async Task CheckProductCount(HttpClient httpClient, string baseUrl)
        {
            try
            {
                var response = await httpClient.GetAsync($"{baseUrl}/api/products/category/{Id}");

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    };
                    var products = JsonSerializer.Deserialize<List<ProductDto>>(content, options);
                    ProductCount = products?.Count ?? 0;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking product count");
            }
        }
    }
}
