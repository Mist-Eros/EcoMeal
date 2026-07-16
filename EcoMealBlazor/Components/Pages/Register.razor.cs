using EcoMeal.EcoMealBlazor.Models.Auth;
using EcoMeal.EcoMealBlazor.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using EcoMeal.EcoMealBlazor.Resources;
using System;
using System.Threading.Tasks;

namespace EcoMeal.Components.Pages;

public class RegisterBase : ComponentBase
{
    [Inject]
    protected AuthService AuthService { get; set; } = default!;
    [Inject]
    protected NavigationManager Navigation { get; set; } = default!;
    [Inject]
    protected IStringLocalizer<SharedResource> Loc { get; set; } = default!;


    protected RegisterModel _model = new();
    protected string? _errorMessage;
    protected bool _isSubmitting;
    protected bool _showPassword;

    protected void TogglePassword() { _showPassword = !_showPassword; }
    protected void GoToLogin() { Navigation.NavigateTo("/login"); }

    protected async Task HandleRegister()
    {
        _isSubmitting = true;
        _errorMessage = null;

        var result = await AuthService.RegisterAsync(
            _model.Email, _model.Password, _model.Name, _model.Contact);

        _isSubmitting = false;

        if (result.Success)
            Navigation.NavigateTo("/login?registered=true");
        else
            _errorMessage = result.ErrorMessage;
    }
}
