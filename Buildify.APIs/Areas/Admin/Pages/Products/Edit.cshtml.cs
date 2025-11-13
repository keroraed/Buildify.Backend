using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Buildify.Core.DTOs;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace Buildify.APIs.Areas.Admin.Pages.Products
{
    public class EditModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<EditModel> _logger;
        private readonly IWebHostEnvironment _environment;

        public EditModel(IHttpClientFactory httpClientFactory, ILogger<EditModel> logger, IWebHostEnvironment environment)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
            _environment = environment;
        }

        [BindProperty]
        public UpdateProductDto Product { get; set; } = new();

        [BindProperty]
        public IFormFile? ImageFile { get; set; }
        
        [BindProperty(SupportsGet = true)]
        public int Id { get; set; }
        
        public List<CategoryDto> Categories { get; set; } = new();
        public string? ErrorMessage { get; set; }
        public string? CurrentImageUrl { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            var token = Request.Cookies["AuthToken"];
            
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToPage("/Account/Login", new { area = "Admin", returnUrl = $"/Admin/Products/Edit/{Id}" });
            }

            await LoadCategories(token);
            await LoadProduct(token);
            
            if (Product == null)
            {
                return RedirectToPage("./Index");
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var token = Request.Cookies["AuthToken"];
            
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToPage("/Account/Login", new { area = "Admin", returnUrl = $"/Admin/Products/Edit/{Id}" });
            }

            if (!ModelState.IsValid)
            {
                await LoadCategories(token);
                return Page();
            }

            try
            {
                // Handle image upload if provided
                if (ImageFile != null && ImageFile.Length > 0)
                {
                    var uploadResult = await UploadImageAsync(ImageFile);
                    if (uploadResult.Success)
                    {
                        // Delete old image if it exists and is a local file
                        if (!string.IsNullOrEmpty(Product.ImageUrl) && Product.ImageUrl.StartsWith("/images/"))
                        {
                            DeleteOldImage(Product.ImageUrl);
                        }
                        
                        Product.ImageUrl = uploadResult.ImagePath;
                    }
                    else
                    {
                        ErrorMessage = uploadResult.ErrorMessage;
                        await LoadCategories(token);
                        return Page();
                    }
                }

                var httpClient = _httpClientFactory.CreateClient();
                var baseUrl = $"{Request.Scheme}://{Request.Host}";
                
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var json = JsonSerializer.Serialize(Product);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await httpClient.PutAsync($"{baseUrl}/api/products/{Id}", content);

                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = "Product updated successfully!";
                    return RedirectToPage("./Index");
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    _logger.LogWarning("Unauthorized access - token invalid or expired");
                    Response.Cookies.Delete("AuthToken");
                    return RedirectToPage("/Account/Login", new { area = "Admin", returnUrl = $"/Admin/Products/Edit/{Id}" });
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    ErrorMessage = $"Failed to update product. Status: {response.StatusCode}";
                    _logger.LogError("Failed to update product. Status: {StatusCode}, Error: {Error}", response.StatusCode, errorContent);
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = "An error occurred while updating the product.";
                _logger.LogError(ex, "Error updating product");
            }

            await LoadCategories(token);
            return Page();
        }

        private async Task LoadProduct(string token)
        {
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
                    var productDto = JsonSerializer.Deserialize<ProductDto>(content, options);
                    
                    if (productDto != null)
                    {
                        Product = new UpdateProductDto
                        {
                            Name = productDto.Name,
                            Description = productDto.Description,
                            Price = productDto.Price,
                            Stock = productDto.Stock,
                            CategoryId = productDto.CategoryId,
                            ImageUrl = productDto.ImageUrl
                        };
                        CurrentImageUrl = productDto.ImageUrl;
                    }
                }
                else
                {
                    ErrorMessage = "Product not found.";
                    _logger.LogError("Failed to load product. Status code: {StatusCode}", response.StatusCode);
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = "Error loading product.";
                _logger.LogError(ex, "Error loading product");
            }
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

        private async Task<(bool Success, string? ImagePath, string? ErrorMessage)> UploadImageAsync(IFormFile imageFile)
        {
            try
            {
                // Validate file type
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
                var fileExtension = Path.GetExtension(imageFile.FileName).ToLowerInvariant();
                
                if (!allowedExtensions.Contains(fileExtension))
                {
                    return (false, null, "Invalid file type. Only JPG, PNG, GIF, and WEBP images are allowed.");
                }

                // Validate file size (max 5MB)
                if (imageFile.Length > 5 * 1024 * 1024)
                {
                    return (false, null, "File size exceeds 5MB limit.");
                }

                // Create unique filename
                var fileName = $"{Guid.NewGuid()}{fileExtension}";
                var uploadsFolder = Path.Combine(_environment.WebRootPath, "images", "products");
                
                // Create directory if it doesn't exist
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                var filePath = Path.Combine(uploadsFolder, fileName);

                // Save file
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await imageFile.CopyToAsync(fileStream);
                }

                // Return relative path for database
                var relativePath = $"/images/products/{fileName}";
                return (true, relativePath, null);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading image");
                return (false, null, "An error occurred while uploading the image.");
            }
        }

        private void DeleteOldImage(string imagePath)
        {
            try
            {
                var filePath = Path.Combine(_environment.WebRootPath, imagePath.TrimStart('/'));
                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                    _logger.LogInformation("Deleted old image: {FilePath}", filePath);
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to delete old image: {ImagePath}", imagePath);
            }
        }
    }
}
