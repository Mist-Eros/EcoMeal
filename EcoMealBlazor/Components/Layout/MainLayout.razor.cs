using EcoMeal.EcoMealBlazor.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace EcoMeal.Components.Layout;

public class MainLayoutBase : LayoutComponentBase
{
    [Inject]
    protected AuthService AuthService { get; set; } = default!;
    [Inject]
    protected NavigationManager Navigation { get; set; } = default!;
    [Inject]
    protected IJSRuntime JS { get; set; } = default!;


    protected bool _isHomePage = true;

    protected override void OnInitialized()
    {
        AuthService.OnChange += StateHasChanged;
        Navigation.LocationChanged += OnLocationChanged;
        _isHomePage = IsHomePageUri(Navigation.Uri);
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            try
            {
                await AuthService.LoadTokenAsync();
                StateHasChanged();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading auth token: {ex.Message}");
            }
        }
    }

    protected void OnLocationChanged(object? sender, Microsoft.AspNetCore.Components.Routing.LocationChangedEventArgs e)
    {
        _isHomePage = IsHomePageUri(e.Location);
        StateHasChanged();
    }

    protected bool IsHomePageUri(string uri) =>
        uri == Navigation.BaseUri || uri == Navigation.BaseUri.TrimEnd('/');

    protected async void GoBack()
    {
        await JS.InvokeVoidAsync("history.back");
    }

    public void Dispose()
    {
        AuthService.OnChange -= StateHasChanged;
        Navigation.LocationChanged -= OnLocationChanged;
    }
}
