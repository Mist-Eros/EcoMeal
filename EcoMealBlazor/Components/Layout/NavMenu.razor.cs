using System.Security.Claims;
using EcoMeal.EcoMealBlazor.Models;
using EcoMeal.EcoMealBlazor.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Localization;
using EcoMeal.EcoMealBlazor.Resources;
using Microsoft.JSInterop;

namespace EcoMeal.Components.Layout;

public class NavMenuBase : LayoutComponentBase
{
    [Inject]
    protected AuthService AuthService { get; set; } = default!;
    [Inject]
    protected AuthenticationStateProvider AuthStateProvider { get; set; } = default!;
    [Inject]
    protected DarkModeService DarkModeService { get; set; } = default!;
    [Inject]
    protected IJSRuntime JSRuntime { get; set; } = default!;
    [Inject]
    protected NavigationManager NavigationManager { get; set; } = default!;
    [Inject]
    protected IStringLocalizer<SharedResource> Loc { get; set; } = default!;


    protected bool _isLoading = true;
    protected string _userName = string.Empty;
    protected string _userRole = string.Empty;

    protected override async Task OnInitializedAsync()
    {
        AuthService.OnChange += OnAuthStateChanged;
        DarkModeService.OnChange += OnThemeChanged;
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            try
            {
                await AuthService.LoadTokenAsync();
                await LoadUserInfo();

                var savedTheme = await JSRuntime.InvokeAsync<string>("localStorage.getItem", "theme");
                if (!string.IsNullOrEmpty(savedTheme))
                {
                    DarkModeService.IsDarkMode = savedTheme == "dark";
                    await JSRuntime.InvokeVoidAsync("applyTheme", DarkModeService.IsDarkMode);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading auth token: {ex.Message}");
            }
            finally
            {
                _isLoading = false;
                StateHasChanged();
            }
        }
    }

    protected void GoToAccount()
    {
        NavigationManager.NavigateTo("/account");
    }

    protected void GoToLanguages()
    {
        NavigationManager.NavigateTo("/languages");
    }

    protected async void OnAuthStateChanged()
    {
        try
        {
            await LoadUserInfo();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in OnAuthStateChanged: {ex.Message}");
        }
        StateHasChanged();
    }

    protected async void OnThemeChanged()
    {
        await InvokeAsync(StateHasChanged);
    }

    protected async Task LoadUserInfo()
    {
        if (AuthService.IsAuthenticated)
        {
            var authState = await AuthStateProvider.GetAuthenticationStateAsync();
            var user = authState.User;

            _userName = user.Identity?.Name ?? "User";

            var roleClaim = user.FindFirst(ClaimTypes.Role);
            _userRole = roleClaim?.Value ?? UserRoles.User;
        }
        else
        {
            _userName = string.Empty;
            _userRole = string.Empty;
        }
    }

    protected async Task ToggleDarkMode()
    {
        DarkModeService.Toggle();
        var theme = DarkModeService.IsDarkMode ? "dark" : "light";
        await JSRuntime.InvokeVoidAsync("applyTheme", DarkModeService.IsDarkMode);
        await JSRuntime.InvokeVoidAsync("localStorage.setItem", "theme", theme);
    }

    public void Dispose()
    {
        AuthService.OnChange -= OnAuthStateChanged;
        DarkModeService.OnChange -= OnThemeChanged;
    }
}
