using System.Net.Http.Json;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using EcoMeal.EcoMealBlazor.Models.Auth;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;

namespace EcoMeal.EcoMealBlazor.Services;

// manages JWT token in localStorage, login/logout/register
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
        var response = await _http.PostAsJsonAsync("api/auth/register", request);

        if (response.IsSuccessStatusCode)
            return AuthResult.Ok();

        var error = await response.Content.ReadAsStringAsync();
        return AuthResult.Fail(error);
    }

    // POST /login — stores token on success
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

                var roles = DecodeRolesFromToken(Token);
                var name = DecodeNameFromToken(Token);
                await _localStorage.SetAsync("userRoles", roles);
                await _localStorage.SetAsync("userName", name);

                if (_authStateProvider is CustomAuthenticationStateProvider customProvider)
                {
                    customProvider.NotifyUserAuthentication(Token, roles, name);
                }
                
                NotifyStateChanged();
            }

            return AuthResult.Ok();
        }

        return AuthResult.Fail("Invalid email or password.");
    }

    public async Task LoadTokenAsync()
    {
        try
        {
            var tokenResult = await _localStorage.GetAsync<string>("authToken");
            Token = tokenResult.Success ? tokenResult.Value : null;

            if (Token != null)
            {
                var rolesResult = await _localStorage.GetAsync<List<string>>("userRoles");
                var roles = rolesResult.Success && rolesResult.Value != null ? rolesResult.Value : new List<string>();
                var nameResult = await _localStorage.GetAsync<string>("userName");
                var name = nameResult.Success && !string.IsNullOrEmpty(nameResult.Value) ? nameResult.Value : "User";

                if (_authStateProvider is CustomAuthenticationStateProvider customProvider)
                {
                    customProvider.NotifyUserAuthentication(Token, roles, name);
                }
            }
        }
        catch (CryptographicException)
        {
            Token = null;
            await _localStorage.DeleteAsync("authToken");
            await _localStorage.DeleteAsync("userRoles");
        }
    }

    public async Task LogoutAsync()
    {
        Token = null;
        await _localStorage.DeleteAsync("authToken");
        await _localStorage.DeleteAsync("userRoles");
        await _localStorage.DeleteAsync("userEmail");
        await _localStorage.DeleteAsync("userName");

        if (_authStateProvider is CustomAuthenticationStateProvider customProvider)
        {
            customProvider.NotifyUserLogout();
        }
        
        NotifyStateChanged();
    }

    public async Task<string?> GetTokenAsync()
    {
        if (!string.IsNullOrEmpty(Token))
            return Token;
        
        await LoadTokenAsync();
        return Token;
    }

    public async Task UpdateRolesAsync(List<string> roles)
    {
        await _localStorage.SetAsync("userRoles", roles);
    }

    public async Task<bool> DeleteAccountAsync()
    {
        var request = new HttpRequestMessage(HttpMethod.Delete, "api/auth/me");
        await Task.Run(() => {});
        if (string.IsNullOrEmpty(Token))
            await LoadTokenAsync();

        if (!string.IsNullOrEmpty(Token))
        {
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", Token);
        }

        var response = await _http.SendAsync(request);
        return response.IsSuccessStatusCode;
    }

    public void NotifyChanged()
    {
        NotifyStateChanged();
    }

    private void NotifyStateChanged() => OnChange?.Invoke();

    private static List<string> DecodeRolesFromToken(string token)
    {
        var parts = token.Split('.');
        if (parts.Length < 2)
            return new List<string> { "User" };

        var payload = parts[1];
        var mod = payload.Length % 4;
        if (mod > 0)
            payload += new string('=', 4 - mod);

        try
        {
            var jsonBytes = Convert.FromBase64String(payload.Replace('-', '+').Replace('_', '/'));
            var json = Encoding.UTF8.GetString(jsonBytes);
            using var doc = JsonDocument.Parse(json);
            var roles = new List<string>();

            var roleClaimType = "http://schemas.microsoft.com/ws/2008/06/identity/claims/role";
            if (doc.RootElement.TryGetProperty(roleClaimType, out var roleEl))
            {
                if (roleEl.ValueKind == JsonValueKind.Array)
                    roles.AddRange(roleEl.EnumerateArray().Select(r => r.GetString()!));
                else
                    roles.Add(roleEl.GetString()!);
            }

            return roles.Count > 0 ? roles : new List<string> { "User" };
        }
        catch
        {
            return new List<string> { "User" };
        }
    }

    private static string DecodeNameFromToken(string token)
    {
        var parts = token.Split('.');
        if (parts.Length < 2)
            return "User";

        var payload = parts[1];
        var mod = payload.Length % 4;
        if (mod > 0)
            payload += new string('=', 4 - mod);

        try
        {
            var jsonBytes = Convert.FromBase64String(payload.Replace('-', '+').Replace('_', '/'));
            var json = Encoding.UTF8.GetString(jsonBytes);
            using var doc = JsonDocument.Parse(json);

            var nameClaimType = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name";
            if (doc.RootElement.TryGetProperty(nameClaimType, out var nameEl))
                return nameEl.GetString() ?? "User";

            return "User";
        }
        catch
        {
            return "User";
        }
    }
}