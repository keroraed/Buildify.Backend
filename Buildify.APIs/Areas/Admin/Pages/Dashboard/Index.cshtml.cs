using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Buildify.Core.DTOs;
using System.Net.Http.Headers;
using System.Text.Json;

namespace Buildify.APIs.Areas.Admin.Pages.Dashboard
{
    public class IndexModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<IndexModel> _logger;

        public IndexModel(IHttpClientFactory httpClientFactory, ILogger<IndexModel> logger)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
        }

        public DashboardStatsDto? DashboardStats { get; set; }
        public string? ErrorMessage { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            // Get JWT token from cookie
            var token = Request.Cookies["AuthToken"];
            
            // If no token, redirect to login
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToPage("/Account/Login", new { area = "Admin", returnUrl = "/Admin/Dashboard" });
            }

            try
            {
                var httpClient = _httpClientFactory.CreateClient();
                
                // Get the base URL from the request
                var baseUrl = $"{Request.Scheme}://{Request.Host}";
                
                // Add JWT token to request header
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                // Call the dashboard API endpoint
                var response = await httpClient.GetAsync($"{baseUrl}/api/dashboard/stats");

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    };
                    DashboardStats = JsonSerializer.Deserialize<DashboardStatsDto>(content, options);
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    // Token expired or invalid, redirect to login
                    _logger.LogWarning("Unauthorized access - token invalid or expired");
                    Response.Cookies.Delete("AuthToken");
                    return RedirectToPage("/Account/Login", new { area = "Admin", returnUrl = "/Admin/Dashboard" });
                }
                else
                {
                    ErrorMessage = $"Failed to load dashboard data. Status: {response.StatusCode}";
                    _logger.LogError("Failed to load dashboard stats. Status code: {StatusCode}", response.StatusCode);
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = "An error occurred while loading the dashboard data.";
                _logger.LogError(ex, "Error loading dashboard stats");
            }

            return Page();
        }
    }
}
