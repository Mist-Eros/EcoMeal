using EcoMeal.EcoMealBlazor.Models;
using EcoMeal.EcoMealBlazor.Services;
using Microsoft.AspNetCore.Components;

namespace EcoMeal.EcoMealBlazor.Components.BusinessList;

public partial class BusinessList
{
    [Inject] 
    public required BusinessService BusinessService { get; set; }
    
    private List<BusinessModel>? Businesses { get; set; }
    
    protected override async Task OnInitializedAsync()
    {
        await RefreshList();
    }

    private async Task RefreshList()
    {
        Businesses = await BusinessService.GetAllAsync();
        StateHasChanged();
    }
}