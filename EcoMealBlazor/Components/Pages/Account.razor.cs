using EcoMeal.EcoMealBlazor.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Localization;
using EcoMeal.EcoMealBlazor.Resources;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Security.Claims;
using System.Threading.Tasks;

namespace EcoMeal.Components.Pages;

public class AccountBase : ComponentBase
{
    [Inject]
    protected AuthService AuthService { get; set; } = default!;
    [Inject]
    protected AuthenticationStateProvider AuthStateProvider { get; set; } = default!;
    [Inject]
    protected NavigationManager Navigation { get; set; } = default!;
    [Inject]
    protected HttpClient Http { get; set; } = default!;
    [Inject]
    protected OrderService OrderService { get; set; } = default!;
    [Inject]
    protected IJSRuntime JSRuntime { get; set; } = default!;
    [Inject]
    protected IStringLocalizer<SharedResource> Loc { get; set; } = default!;


    protected bool _isLoading = true;
    protected bool _isEditingRole = false;
    protected bool _roleUpdateSuccess = false;
    protected string _userEmail = string.Empty;
    protected string _userName = string.Empty;
    protected string _userRole = string.Empty;
    protected string _selectedRole = string.Empty;
    protected string _roleUpdateMessage = string.Empty;
    protected List<string> _availableRoles = new() { "User", "Admin" };
    protected bool _isClearing = false;
    protected string _clearMessage = string.Empty;
    protected bool _clearSuccess = false;
    protected bool _isDeleting = false;

    protected bool _showConfirm = false;
    protected string _confirmMessage = "";
    protected Func<Task>? _confirmAction = null;

    protected override async Task OnInitializedAsync()
    {
        try
        {
            await LoadUserInfo();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading account: {ex.Message}");
        }
        finally
        {
            _isLoading = false;
            StateHasChanged();
        }
    }

    protected async Task LoadUserInfo()
    {
        try
        {
            var authState = await AuthStateProvider.GetAuthenticationStateAsync();
            var user = authState.User;
            
            if (user.Identity?.IsAuthenticated == true)
            {
                _userEmail = user.FindFirst(ClaimTypes.Email)?.Value ?? "Unknown";
                _userName = user.FindFirst(ClaimTypes.Name)?.Value ?? "User";
                _userRole = user.FindFirst(ClaimTypes.Role)?.Value ?? "User";
                _selectedRole = _userRole;
            }
            else
            {
                Navigation.NavigateTo("/login");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading user info: {ex.Message}");
        }
    }

    protected void GoToHome()
    {
        Navigation.NavigateTo("/");
    }

    protected void ToggleEditRole()
    {
        _isEditingRole = !_isEditingRole;
        if (_isEditingRole)
        {
            _selectedRole = _userRole;
            _roleUpdateMessage = string.Empty;
        }
        StateHasChanged();
    }

    protected async Task SelectRole(string role)
    {
        if (role == _userRole) return;

        _selectedRole = role;
        _roleUpdateMessage = string.Empty;
        StateHasChanged();

        var token = await AuthService.GetTokenAsync();
        Http.DefaultRequestHeaders.Authorization = 
            new AuthenticationHeaderValue("Bearer", token);

        var encodedEmail = Uri.EscapeDataString(_userEmail);

        try
        {
            var response = await Http.PutAsJsonAsync($"api/users/{encodedEmail}/role", new { Role = role });

            if (response.IsSuccessStatusCode)
            {
                _roleUpdateSuccess = true;
                _roleUpdateMessage = string.Format(Loc["Role updated"].Value, role);
                _userRole = role;

                await AuthService.UpdateRolesAsync(new List<string> { role });
                AuthService.NotifyChanged();

                StateHasChanged();
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                _roleUpdateSuccess = false;

                try
                {
                    var errorObj = JsonSerializer.Deserialize<ErrorResponse>(errorContent);
                    _roleUpdateMessage = errorObj?.Message ?? errorContent;
                }
                catch
                {
                    _roleUpdateMessage = errorContent;
                }
            }
        }
        catch (Exception ex)
        {
            _roleUpdateSuccess = false;
            _roleUpdateMessage = $"Error: {ex.Message}";
        }
    }

    protected async Task HandleLogout()
    {
        await AuthService.LogoutAsync();
        Navigation.NavigateTo("/", forceLoad: true);
    }

    protected void HandleDeleteAccount()
    {
        _confirmMessage = Loc["Confirm delete account"].Value;
        _confirmAction = ExecuteDeleteAccount;
        _showConfirm = true;
    }

    protected async Task ExecuteDeleteAccount()
    {
        _showConfirm = false;
        _isDeleting = true;
        StateHasChanged();

        var success = await AuthService.DeleteAccountAsync();
        if (success)
        {
            await AuthService.LogoutAsync();
            Navigation.NavigateTo("/", forceLoad: true);
        }
        else
        {
            _isDeleting = false;
            StateHasChanged();
        }
    }

    protected void HandleClearOrders()
    {
        _confirmMessage = Loc["Confirm clear orders"].Value;
        _confirmAction = ExecuteClearOrders;
        _showConfirm = true;
    }

    protected async Task ExecuteClearOrders()
    {
        _showConfirm = false;
        _isClearing = true;
        _clearMessage = string.Empty;
        StateHasChanged();

        var success = await OrderService.ClearOrderHistoryAsync();

        _isClearing = false;
        _clearSuccess = success;
        _clearMessage = success
            ? Loc["Order history cleared"].Value
            : Loc["Failed to clear orders"].Value;
        StateHasChanged();
    }

    protected async Task ExecuteConfirm()
    {
        if (_confirmAction != null)
            await _confirmAction();
    }

    protected class ErrorResponse
    {
        public string? Message { get; set; }
    }
}
