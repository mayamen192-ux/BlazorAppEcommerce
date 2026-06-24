using BlazorAppEcommerce.Models;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.Logging;
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
        // LOGIN (COOKIE AUTH)
        // =========================
        public async Task<bool> Login(User user)
        {
            try
            {
                _logger.LogInformation(
                    "Login attempt | UserID: {UserId} | Username: {Username} | Role: {Role}",
                    user.Id, user.name, user.role);

                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.name),
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Role, user.role)
                };

                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var principal = new ClaimsPrincipal(identity);

                await _httpContextAccessor.HttpContext!
                    .SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

                if (_authStateProvider is CustomAuthenticationStateProvider customProvider)
                {
                    customProvider.MarkUserAsAuthenticated(user.name, user.role);
                    _logger.LogInformation(
                        "Authentication state updated for UserID: {UserId} | Username: {Username}",
                        user.Id, user.name);
                }

                _logger.LogInformation(
                    "User logged in successfully | UserID: {UserId} | Username: {Username} | Role: {Role}",
                    user.Id, user.name, user.role);

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Error occurred during login for UserID: {UserId} | Username: {Username}",
                    user.Id, user.name);

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

                var username = httpContext?.User?.Identity?.Name ?? "Unknown";

                _logger.LogWarning(
                    "Logout initiated for Username: {Username}", username);

                await httpContext!
                    .SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

                if (_authStateProvider is CustomAuthenticationStateProvider customProvider)
                {
                    customProvider.MarkUserAsLoggedOut();
                    _logger.LogInformation(
                        "Authentication state cleared for Username: {Username}", username);
                }

                _logger.LogInformation(
                    "User logged out successfully | Username: {Username}", username);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Error occurred during logout.");
            }
        }
    }
}