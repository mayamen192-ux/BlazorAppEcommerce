using BlazorAppEcommerce.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;

namespace BlazorAppEcommerce.Services
{
    public class AuthService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly AuthenticationStateProvider _authStateProvider;
        private readonly ILogger<AuthService> _logger;

        public AuthService(
            IHttpContextAccessor httpContextAccessor,
            AuthenticationStateProvider authStateProvider,
            ILogger<AuthService> logger)
        {
            _httpContextAccessor = httpContextAccessor;
            _authStateProvider = authStateProvider;
            _logger = logger;
        }

        // =========================
        // LOGIN (COOKIE AUTH - SINGLE SOURCE OF TRUTH)
        // =========================
        public async Task<bool> Login(User user)
        {
            try
            {
                _logger.LogInformation("Login attempt | UserID: {UserId}", user.Id);

                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.name),
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Role, user.role)
                };

                var identity = new ClaimsIdentity(
                    claims,
                    CookieAuthenticationDefaults.AuthenticationScheme);

                var principal = new ClaimsPrincipal(identity);

                // 1. Sign in using cookie authentication
                await _httpContextAccessor.HttpContext!
                    .SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

                // 2. Notify Blazor UI auth state changed
                if (_authStateProvider is CustomAuthenticationStateProvider provider)
                {
                    provider.NotifyAuthStateChanged();
                }

                _logger.LogInformation("Login successful | UserID: {UserId}", user.Id);

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Login failed | UserID: {UserId}", user.Id);
                return false;
            }
        }

        // =========================
        // LOGOUT
        // =========================
        public async Task Logout()
        {
            try
            {
                var httpContext = _httpContextAccessor.HttpContext;

                if (httpContext != null)
                {
                    await httpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                }

                if (_authStateProvider is CustomAuthenticationStateProvider provider)
                {
                    provider.NotifyAuthStateChanged();
                }

                _logger.LogInformation("User logged out successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Logout failed");
            }
        }
    }
}