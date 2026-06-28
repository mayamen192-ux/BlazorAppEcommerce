using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;

public class CustomAuthenticationStateProvider : AuthenticationStateProvider
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CustomAuthenticationStateProvider(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public override Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var user = _httpContextAccessor.HttpContext?.User
                   ?? new ClaimsPrincipal(new ClaimsIdentity());

        return Task.FromResult(new AuthenticationState(user));
    }

    // ✅ PLACE THIS METHOD HERE
    public Task MarkUserAsAuthenticated(string name, string role, int userId)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, name),
            new Claim(ClaimTypes.Role, role),
            new Claim(ClaimTypes.NameIdentifier, userId.ToString())
        };

        var identity = new ClaimsIdentity(claims, "CustomAuth");
        var user = new ClaimsPrincipal(identity);

        _httpContextAccessor.HttpContext!.User = user;

        NotifyAuthenticationStateChanged(
            Task.FromResult(new AuthenticationState(user)));

        return Task.CompletedTask;
    }

    // OPTIONAL (for logout button)
    public void Logout()
    {
        var user = new ClaimsPrincipal(new ClaimsIdentity());

        _httpContextAccessor.HttpContext!.User = user;

        NotifyAuthenticationStateChanged(
            Task.FromResult(new AuthenticationState(user)));
    }

    public void NotifyAuthStateChanged()
    {
        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
    }
}