using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.Extensions.Logging;

namespace EcoMeal.EcoMealBlazor.Services;

public class CustomAuthenticationStateProvider : AuthenticationStateProvider
{
    private readonly ProtectedLocalStorage _localStorage;
    private readonly ILogger<CustomAuthenticationStateProvider> _logger;
    private readonly ClaimsPrincipal _anonymous = new(new ClaimsIdentity());

    public CustomAuthenticationStateProvider(ProtectedLocalStorage localStorage, ILogger<CustomAuthenticationStateProvider> logger)
    {
        _localStorage = localStorage;
        _logger = logger;
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        try
        {
            var tokenResult = await _localStorage.GetAsync<string>("authToken");
            var rolesResult = await _localStorage.GetAsync<List<string>>("userRoles");
            var emailResult = await _localStorage.GetAsync<string>("userEmail");

            if (!tokenResult.Success || string.IsNullOrEmpty(tokenResult.Value))
            {
                return new AuthenticationState(_anonymous);
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, tokenResult.Value),
                new Claim(ClaimTypes.Name, emailResult.Success ? emailResult.Value ?? "user" : "user"),
                new Claim(ClaimTypes.Email, emailResult.Success ? emailResult.Value ?? "user@example.com" : "user@example.com")
            };

            if (rolesResult.Success && rolesResult.Value != null)
            {
                foreach (var role in rolesResult.Value)
                {
                    claims.Add(new Claim(ClaimTypes.Role, role ?? "User"));
                }
            }

            var identity = new ClaimsIdentity(claims, "CustomAuth");
            var user = new ClaimsPrincipal(identity);
            return new AuthenticationState(user);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Failed to retrieve authentication state from local storage during prerendering.");
            return new AuthenticationState(_anonymous);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error retrieving authentication state.");
            return new AuthenticationState(_anonymous);
        }
    }

    public void NotifyUserAuthentication(string token, List<string> roles)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, token ?? string.Empty),
            new Claim(ClaimTypes.Name, "user"),
            //new Claim(ClaimTypes.Email, email ?? "user@example.com")
        };
        
        if (roles != null)
        {
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role ?? "User"));
            }
        }
        else
        {
            claims.Add(new Claim(ClaimTypes.Role, "User"));
        }
        
        var identity = new ClaimsIdentity(claims, "Bearer");
        var user = new ClaimsPrincipal(identity);
        var authState = Task.FromResult(new AuthenticationState(user));
        NotifyAuthenticationStateChanged(authState);
    }

    public void NotifyUserLogout()
    {
        var authState = Task.FromResult(new AuthenticationState(_anonymous));
        NotifyAuthenticationStateChanged(authState);
    }
}