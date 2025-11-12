using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Buildify.Core.DTOs;
using System.Net.Http.Headers;
using System.Text.Json;

namespace Buildify.APIs.Areas.Admin.Pages.Products
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
        
        public ProductDto? Product { get; set; }
        public string? ErrorMessage { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            var token = Request.Cookies["AuthToken"];
            
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToPage("/Account/Login", new { area = "Admin", returnUrl = $"/Admin/Products/Details/{Id}" });
            }

            try
            {
                var httpClient = _httpClientFactory.CreateClient();
                var baseUrl = $"{Request.Scheme}://{Request.Host}";
                
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var response = await httpClient.GetAsync($"{baseUrl}/api/products/{Id}");

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    };
                    Product = JsonSerializer.Deserialize<ProductDto>(content, options);
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    _logger.LogWarning("Unauthorized access - token invalid or expired");
                    Response.Cookies.Delete("AuthToken");
                    return RedirectToPage("/Account/Login", new { area = "Admin", returnUrl = $"/Admin/Products/Details/{Id}" });
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    ErrorMessage = "Product not found.";
                    _logger.LogError("Product not found. ID: {Id}", Id);
                }
                else
                {
                    ErrorMessage = $"Failed to load product. Status: {response.StatusCode}";
                    _logger.LogError("Failed to load product. Status code: {StatusCode}", response.StatusCode);
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = "An error occurred while loading the product.";
                _logger.LogError(ex, "Error loading product");
            }

            return Page();
        }
    }
}
