using BlazorAppEcommerce.Models;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace BlazorAppEcommerce.Services
{
    public class AuthService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly AuthenticationStateProvider _authStateProvider;

        public AuthService(
            IHttpContextAccessor httpContextAccessor,
            AuthenticationStateProvider authStateProvider)
        {
            _httpContextAccessor = httpContextAccessor;
            _authStateProvider = authStateProvider;
        }

        // =========================
        // LOGIN (COOKIE AUTH)
        // =========================
        public async Task<bool> Login(User user)
        {
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
            }

            return true;
        }

        // =========================
        // LOGOUT
        // =========================
        public async Task Logout()
        {
            await _httpContextAccessor.HttpContext!
                .SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            if (_authStateProvider is CustomAuthenticationStateProvider customProvider)
            {
                customProvider.MarkUserAsLoggedOut();
            }
        }
    }
}