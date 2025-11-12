using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Buildify.Core.DTOs;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace Buildify.APIs.Areas.Admin.Pages.Products
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
        public CreateProductDto Product { get; set; } = new();
        
        public List<CategoryDto> Categories { get; set; } = new();
        public string? ErrorMessage { get; set; }
        public string? SuccessMessage { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            var token = Request.Cookies["AuthToken"];
            
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToPage("/Account/Login", new { area = "Admin", returnUrl = "/Admin/Products/Create" });
            }

            await LoadCategories(token);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var token = Request.Cookies["AuthToken"];
            
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToPage("/Account/Login", new { area = "Admin", returnUrl = "/Admin/Products/Create" });
            }

            if (!ModelState.IsValid)
            {
                await LoadCategories(token);
                return Page();
            }

            try
            {
                var httpClient = _httpClientFactory.CreateClient();
                var baseUrl = $"{Request.Scheme}://{Request.Host}";
                
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var json = JsonSerializer.Serialize(Product);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await httpClient.PostAsync($"{baseUrl}/api/products", content);

                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = "Product created successfully!";
                    return RedirectToPage("./Index");
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    _logger.LogWarning("Unauthorized access - token invalid or expired");
                    Response.Cookies.Delete("AuthToken");
                    return RedirectToPage("/Account/Login", new { area = "Admin", returnUrl = "/Admin/Products/Create" });
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    ErrorMessage = $"Failed to create product. Status: {response.StatusCode}";
                    _logger.LogError("Failed to create product. Status: {StatusCode}, Error: {Error}", response.StatusCode, errorContent);
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = "An error occurred while creating the product.";
                _logger.LogError(ex, "Error creating product");
            }

            await LoadCategories(token);
            return Page();
        }

        private async Task LoadCategories(string token)
        {
            try
            {
                var httpClient = _httpClientFactory.CreateClient();
                var baseUrl = $"{Request.Scheme}://{Request.Host}";
                
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var response = await httpClient.GetAsync($"{baseUrl}/api/categories");

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    };
                    Categories = JsonSerializer.Deserialize<List<CategoryDto>>(content, options) ?? new List<CategoryDto>();
                }
                else
                {
                    _logger.LogError("Failed to load categories. Status code: {StatusCode}", response.StatusCode);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading categories");
            }
        }
    }
}
