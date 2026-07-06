using Ecomeal.EcoMealBlazor.Models;
using EcoMeal.EcoMealBlazor.Services;
using Microsoft.AspNetCore.Components;

namespace EcoMeal.EcoMealBlazor.Components.BusinessCard;

public partial class BusinessCard
{
    [Parameter]
    public required BusinessModel Business { get; set;}
    [Inject]
    public required BusinessService BusinessService { get; set;}
    
    [Parameter]
    public EventCallback OnDeleted { get; set; }

    private bool isDeleting = false;

    private async Task DeleteBusiness()
    {
        if (isDeleting) return;
        
        isDeleting = true;
        
        var success = await BusinessService.DeleteAsync(Business.Id);
        
        if (success)
        {
            await OnDeleted.InvokeAsync();
        }
        
        isDeleting = false;
    }
}