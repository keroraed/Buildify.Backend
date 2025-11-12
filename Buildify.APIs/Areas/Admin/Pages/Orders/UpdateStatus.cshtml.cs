using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Buildify.Core.DTOs;
using Buildify.Core.Entities;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace Buildify.APIs.Areas.Admin.Pages.Orders
{
    public class UpdateStatusModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<UpdateStatusModel> _logger;

        public UpdateStatusModel(IHttpClientFactory httpClientFactory, ILogger<UpdateStatusModel> logger)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
        }

        [BindProperty(SupportsGet = true)]
        public int Id { get; set; }
        
        [BindProperty]
        public OrderStatus NewStatus { get; set; }
        
        public OrderDto? Order { get; set; }
        public string? ErrorMessage { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            var token = Request.Cookies["AuthToken"];
            
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToPage("/Account/Login", new { area = "Admin", returnUrl = $"/Admin/Orders/UpdateStatus/{Id}" });
            }

            await LoadOrder(token);
            
            if (Order == null)
            {
                return RedirectToPage("./Index");
            }

            // Set current status as default
            if (Enum.TryParse<OrderStatus>(Order.Status, out var currentStatus))
            {
                NewStatus = currentStatus;
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var token = Request.Cookies["AuthToken"];
            
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToPage("/Account/Login", new { area = "Admin", returnUrl = $"/Admin/Orders/UpdateStatus/{Id}" });
            }

            if (!ModelState.IsValid)
            {
                await LoadOrder(token);
                return Page();
            }

            try
            {
                var httpClient = _httpClientFactory.CreateClient();
                var baseUrl = $"{Request.Scheme}://{Request.Host}";
                
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var updateDto = new UpdateOrderStatusDto { Status = NewStatus };
                var json = JsonSerializer.Serialize(updateDto);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await httpClient.PutAsync($"{baseUrl}/api/orders/{Id}/status", content);

                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = "Order status updated successfully!";
                    return RedirectToPage("./Details", new { id = Id });
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    _logger.LogWarning("Unauthorized access - token invalid or expired");
                    Response.Cookies.Delete("AuthToken");
                    return RedirectToPage("/Account/Login", new { area = "Admin", returnUrl = $"/Admin/Orders/UpdateStatus/{Id}" });
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    ErrorMessage = $"Failed to update order status. Status: {response.StatusCode}";
                    _logger.LogError("Failed to update order status. Status: {StatusCode}, Error: {Error}", response.StatusCode, errorContent);
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = "An error occurred while updating the order status.";
                _logger.LogError(ex, "Error updating order status");
            }

            await LoadOrder(token);
            return Page();
        }

        private async Task LoadOrder(string token)
        {
            try
            {
                var httpClient = _httpClientFactory.CreateClient();
                var baseUrl = $"{Request.Scheme}://{Request.Host}";
                
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var response = await httpClient.GetAsync($"{baseUrl}/api/orders/{Id}");

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    };
                    Order = JsonSerializer.Deserialize<OrderDto>(content, options);
                }
                else
                {
                    ErrorMessage = "Order not found.";
                    _logger.LogError("Failed to load order. Status code: {StatusCode}", response.StatusCode);
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = "Error loading order.";
                _logger.LogError(ex, "Error loading order");
            }
        }
    }
}
