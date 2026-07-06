using Ecomeal.EcoMealBlazor.Models;
using EcoMeal.EcoMealBlazor.Services;
using Microsoft.AspNetCore.Components;

namespace EcoMeal.EcoMealBlazor.Components.BusinessList;

public partial class BusinessList
{
    [Inject] 
    public required BusinessService BusinessService { get; set;}
    private List<BusinessModel>? Business { get; set;}
    protected override async Task OnInitializedAsync()
    {
        Business = await BusinessService.GetAllAsync();
    }
}