using EcoMeal.EcoMealBlazor.Services;
using Microsoft.Extensions.Localization;
using EcoMeal.EcoMealBlazor.Resources;
using Microsoft.AspNetCore.Components;

namespace EcoMeal.Components.Pages;

public class OrderConfirmedBase : ComponentBase
{
    [Inject]
    protected OrderService OrderService { get; set; } = default!;
    [Inject]
    protected NavigationManager NavigationManager { get; set; } = default!;
    [Inject]
    protected IStringLocalizer<SharedResource> Loc { get; set; } = default!;


    [Parameter]
    public int BusinessId { get; set; }

    [Parameter]
    public int PackageId { get; set; }

    protected bool _isPlacing = true;
    protected bool _success = false;
    protected string _errorMessage = "";

    protected override async Task OnInitializedAsync()
    {
        try
        {
            _success = await OrderService.PlaceOrderAsync(PackageId);
        }
        catch (Exception)
        {
            _success = false;
            _errorMessage = Loc["Could not place order"].Value;
        }
        finally
        {
            _isPlacing = false;
        }
    }

    protected void GoBack()
    {
        NavigationManager.NavigateTo($"/business/{BusinessId}");
    }
}
