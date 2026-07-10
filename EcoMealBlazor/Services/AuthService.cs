using System.Net.Http.Json;
using EcoMeal.EcoMealBlazor.Models.Auth;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;

namespace EcoMeal.EcoMealBlazor.Services;

public class AuthService
{
    private readonly HttpClient _http;
    private readonly ProtectedLocalStorage _localStorage;
    private readonly AuthenticationStateProvider _authStateProvider;

    public string? Token { get; private set; }
    public bool IsAuthenticated => !string.IsNullOrEmpty(Token);
    
    public event Action? OnChange;

    public AuthService(HttpClient http, ProtectedLocalStorage localStorage, AuthenticationStateProvider authStateProvider)
    {
        _http = http;
        _localStorage = localStorage;
        _authStateProvider = authStateProvider;
    }

    public async Task<AuthResult> RegisterAsync(string email, string password, string name, string contact)
    {
        var request = new RegisterRequest { Email = email, Password = password, Name = name, Contact = contact };
        var response = await _http.PostAsJsonAsync("register", request);

        if (response.IsSuccessStatusCode)
            return AuthResult.Ok();

        var error = await response.Content.ReadAsStringAsync();
        return AuthResult.Fail(error);
    }

    public async Task<AuthResult> LoginAsync(string email, string password)
    {
        var request = new AuthRequest { Email = email, Password = password };
        var response = await _http.PostAsJsonAsync("login", request);

        if (response.IsSuccessStatusCode)
        {
            var result = await response.Content.ReadFromJsonAsync<LoginResponse>();
            Token = result?.AccessToken;

            if (Token != null)
            {
                await _localStorage.SetAsync("authToken", Token);
                await _localStorage.SetAsync("userEmail", email);

                var roles = new List<string> { "User" };
                await _localStorage.SetAsync("userRoles", roles);

                if (_authStateProvider is CustomAuthenticationStateProvider customProvider)
                {
                    customProvider.NotifyUserAuthentication(Token, roles, email);
                }
                
                NotifyStateChanged();
            }

            return AuthResult.Ok();
        }

        return AuthResult.Fail("Invalid email or password.");
    }

    public async Task LoadTokenAsync()
    {
        var tokenResult = await _localStorage.GetAsync<string>("authToken");
        Token = tokenResult.Success ? tokenResult.Value : null;

        if (Token != null)
        {
            var rolesResult = await _localStorage.GetAsync<List<string>>("userRoles");
            var roles = rolesResult.Success && rolesResult.Value != null ? rolesResult.Value : new List<string>();

            if (_authStateProvider is CustomAuthenticationStateProvider customProvider)
            {
                var emailResult = await _localStorage.GetAsync<string>("userEmail");
                var email = emailResult.Success ? emailResult.Value ?? "user@example.com" : "user@example.com";
                customProvider.NotifyUserAuthentication(Token, roles, email);
            }
        }
        
        NotifyStateChanged();
    }

    public async Task LogoutAsync()
    {
        Token = null;
        await _localStorage.DeleteAsync("authToken");
        await _localStorage.DeleteAsync("userRoles");
        await _localStorage.DeleteAsync("userEmail");

        if (_authStateProvider is CustomAuthenticationStateProvider customProvider)
        {
            customProvider.NotifyUserLogout();
        }
        
        NotifyStateChanged();
    }

    private void NotifyStateChanged() => OnChange?.Invoke();
}