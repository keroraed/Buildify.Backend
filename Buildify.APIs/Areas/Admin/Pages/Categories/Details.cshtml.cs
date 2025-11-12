using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Buildify.Core.DTOs;
using System.Net.Http.Headers;
using System.Text.Json;

namespace Buildify.APIs.Areas.Admin.Pages.Categories
{
    public class DetailsModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<DetailsModel> _logger;

        public DetailsModel(IHttpClientFactory httpClientFactory, ILogger<DetailsModel> logger)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
        }

        [BindProperty(SupportsGet = true)]
        public int Id { get; set; }
        
        public CategoryDto? Category { get; set; }
        public List<ProductDto> CategoryProducts { get; set; } = new();
        public string? ErrorMessage { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            var token = Request.Cookies["AuthToken"];
            
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToPage("/Account/Login", new { area = "Admin", returnUrl = $"/Admin/Categories/Details/{Id}" });
            }

            try
            {
                var httpClient = _httpClientFactory.CreateClient();
                var baseUrl = $"{Request.Scheme}://{Request.Host}";
                
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                // Load category details
                var response = await httpClient.GetAsync($"{baseUrl}/api/categories/{Id}");

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    };
                    Category = JsonSerializer.Deserialize<CategoryDto>(content, options);
                    
                    // Load products in this category
                    await LoadCategoryProducts(httpClient, baseUrl);
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    _logger.LogWarning("Unauthorized access - token invalid or expired");
                    Response.Cookies.Delete("AuthToken");
                    return RedirectToPage("/Account/Login", new { area = "Admin", returnUrl = $"/Admin/Categories/Details/{Id}" });
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    ErrorMessage = "Category not found.";
                    _logger.LogError("Category not found. ID: {Id}", Id);
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

        private async Task LoadCategoryProducts(HttpClient httpClient, string baseUrl)
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
                    CategoryProducts = JsonSerializer.Deserialize<List<ProductDto>>(content, options) ?? new List<ProductDto>();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading category products");
            }
        }
    }
}
