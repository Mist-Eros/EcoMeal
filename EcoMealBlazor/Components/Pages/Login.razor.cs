using EcoMeal.EcoMealBlazor.Models.Auth;
using EcoMeal.EcoMealBlazor.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using EcoMeal.EcoMealBlazor.Resources;
using System;
using System.Threading.Tasks;
using System.Web;

namespace EcoMeal.Components.Pages;

public class LoginBase : ComponentBase
{
    [Inject]
    protected AuthService AuthService { get; set; } = default!;
    [Inject]
    protected NavigationManager Navigation { get; set; } = default!;
    [Inject]
    protected IStringLocalizer<SharedResource> Loc { get; set; } = default!;


    protected LoginModel _model = new();
    protected string? _errorMessage;
    protected bool _isSubmitting;
    protected bool _showPassword;
    protected bool _showSuccess;

    protected override void OnInitialized()
    {
        var uri = new Uri(Navigation.Uri);
        var query = HttpUtility.ParseQueryString(uri.Query);
        _showSuccess = query["registered"] == "true";
    }

    protected void TogglePassword() { _showPassword = !_showPassword; }
    protected void GoToRegister() { Navigation.NavigateTo("/register"); }

    protected async Task HandleLogin()
    {
        _isSubmitting = true;
        _errorMessage = null;
        _showSuccess = false;

        var result = await AuthService.LoginAsync(_model.Email, _model.Password);
        _isSubmitting = false;

        if (result.Success)
            Navigation.NavigateTo("/");
        else
            _errorMessage = Loc["Invalid email or password"];
    }
}
