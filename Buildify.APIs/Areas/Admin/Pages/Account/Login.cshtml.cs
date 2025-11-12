using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace Buildify.APIs.Areas.Admin.Pages.Account
{
    public class LoginModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<LoginModel> _logger;

        public LoginModel(IHttpClientFactory httpClientFactory, ILogger<LoginModel> logger)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
        }

        [BindProperty]
        public LoginInputModel Input { get; set; } = new LoginInputModel();

        public string? ErrorMessage { get; set; }
        public string? ReturnUrl { get; set; }

        public class LoginInputModel
        {
            [Required(ErrorMessage = "Email is required")]
            [EmailAddress(ErrorMessage = "Invalid email address")]
            public string Email { get; set; } = string.Empty;

            [Required(ErrorMessage = "Password is required")]
            [DataType(DataType.Password)]
            public string Password { get; set; } = string.Empty;

            public bool RememberMe { get; set; }
        }

        public void OnGet(string? returnUrl = null)
        {
            ReturnUrl = returnUrl ?? "/Admin/Dashboard";
        }

        public async Task<IActionResult> OnPostAsync(string? returnUrl = null)
        {
            ReturnUrl = returnUrl ?? "/Admin/Dashboard";

            if (!ModelState.IsValid)
            {
                return Page();
            }

            try
            {
                var httpClient = _httpClientFactory.CreateClient();
                var baseUrl = $"{Request.Scheme}://{Request.Host}";

                // Prepare login request
                var loginRequest = new
                {
                    email = Input.Email,
                    password = Input.Password
                };

                var jsonContent = JsonSerializer.Serialize(loginRequest);
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                // Call the login API endpoint
                var response = await httpClient.PostAsync($"{baseUrl}/api/account/login", content);

                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    var options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    };

                    var loginResponse = JsonSerializer.Deserialize<LoginResponse>(responseContent, options);

                    if (loginResponse?.Token != null)
                    {
                        // Store the token in a secure HTTP-only cookie
                        var cookieOptions = new CookieOptions
                        {
                            HttpOnly = true,
                            Secure = true, // HTTPS only
                            SameSite = SameSiteMode.Strict,
                            Expires = Input.RememberMe 
                                ? DateTimeOffset.UtcNow.AddDays(30) 
                                : DateTimeOffset.UtcNow.AddDays(2)
                        };

                        Response.Cookies.Append("AuthToken", loginResponse.Token, cookieOptions);

                        _logger.LogInformation("User {Email} logged in successfully", Input.Email);

                        // Redirect to return URL or dashboard
                        return LocalRedirect(ReturnUrl);
                    }
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    
                    // Try to parse the error message
                    try
                    {
                        var errorResponse = JsonSerializer.Deserialize<ErrorResponse>(errorContent, new JsonSerializerOptions
                        {
                            PropertyNameCaseInsensitive = true
                        });
                        ErrorMessage = errorResponse?.Message ?? "Invalid email or password";
                    }
                    catch
                    {
                        ErrorMessage = "Invalid email or password";
                    }

                    _logger.LogWarning("Failed login attempt for {Email}", Input.Email);
                }
                else
                {
                    ErrorMessage = "An error occurred during login. Please try again.";
                    _logger.LogError("Login failed with status code: {StatusCode}", response.StatusCode);
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = "An unexpected error occurred. Please try again.";
                _logger.LogError(ex, "Exception during login for {Email}", Input.Email);
            }

            return Page();
        }

        private class LoginResponse
        {
            public string? DisplayName { get; set; }
            public string? Email { get; set; }
            public string? Token { get; set; }
        }

        private class ErrorResponse
        {
            public int StatusCode { get; set; }
            public string? Message { get; set; }
        }
    }
}
