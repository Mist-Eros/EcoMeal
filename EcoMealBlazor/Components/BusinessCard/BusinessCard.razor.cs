using EcoMeal.EcoMealBlazor.Models;
using EcoMeal.EcoMealBlazor.Services;
using EcoMeal.EcoMealBlazor.Resources;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Localization;
using Microsoft.JSInterop;

namespace EcoMeal.EcoMealBlazor.Components.BusinessCard;

public partial class BusinessCard
{
    [Parameter]
    public required BusinessModel Business { get; set; }
    
    [Inject]
    public required BusinessService BusinessService { get; set; }
    
    [Inject]
    public required NavigationManager NavigationManager { get; set; }
    
    [Inject]
    public required IJSRuntime JSRuntime { get; set; }
    
    [Inject]
    public required AuthenticationStateProvider AuthStateProvider { get; set; }
    
    [Parameter]
    public EventCallback OnDeleted { get; set; }

    private bool isDeleting = false;
    private bool _isAdmin = false;

    protected override async Task OnInitializedAsync()
    {
        var authState = await AuthStateProvider.GetAuthenticationStateAsync();
        var user = authState.User;
        _isAdmin = user.IsInRole("Admin");
    }

    private string GetBusinessImage()
    {
        var type = Business.BusinessTypeName?.ToLower() ?? "";
        
        if (type.Contains("restaurant"))
            return "https://images.unsplash.com/photo-1517248135467-4c7edcad34c4?w=600&h=300&fit=crop";
        else if (type.Contains("fast food") || type.Contains("fast-food"))
            return "https://images.unsplash.com/photo-1571091718767-18b5b1457add?w=600&h=300&fit=crop";
        else if (type.Contains("bakery"))
            return "https://images.unsplash.com/photo-1555507036-ab1f4038808a?w=600&h=300&fit=crop";
        else
            return "https://images.unsplash.com/photo-1555396273-367ea4eb4db5?w=600&h=300&fit=crop";
    }

    private void NavigateToDetails()
    {
        NavigationManager.NavigateTo($"/business/{Business.Id}");
    }

    private void EditBusiness()
    {
        NavigationManager.NavigateTo($"/business/{Business.Id}/edit");
    }

    private async Task DeleteBusiness()
    {
        if (isDeleting) return;
        
        var confirmed = await JSRuntime.InvokeAsync<bool>("confirm", string.Format(Loc["Confirm delete business"].Value, Business.Name));
        if (!confirmed) return;
        
        isDeleting = true;
        
        var success = await BusinessService.DeleteAsync(Business.Id);
        
        if (success)
        {
            await OnDeleted.InvokeAsync();
        }
        
        isDeleting = false;
    }

    private void OrderBusiness()
    {
        NavigationManager.NavigateTo($"/business/{Business.Id}");
    }

    private async Task ResetRatings()
    {
        var confirmed = await JSRuntime.InvokeAsync<bool>("confirm", Loc["Reset all ratings"].Value);
        if (!confirmed) return;

        var success = await BusinessService.ResetRatingsAsync(Business.Id);
        if (success)
        {
            await OnDeleted.InvokeAsync();
        }
    }

    private string GetTypeIcon()
    {
        var type = Business.BusinessTypeName?.ToLower() ?? "";
        if (type.Contains("restaurant")) return "fa-solid fa-pizza-slice";
        if (type.Contains("fast food") || type.Contains("fast-food")) return "fa-solid fa-burger";
        if (type.Contains("bakery")) return "fa-solid fa-cake-candles";
        return "fa-solid fa-shop";
    }

    private string LocTypeName()
    {
        var t = Business.BusinessTypeName?.ToLower() ?? "";
        if (t.Contains("restaurant")) return Loc["Restaurant"];
        if (t.Contains("fast food") || t.Contains("fast-food")) return Loc["Fast Food"];
        if (t.Contains("bakery")) return Loc["Bakery"];
        return Business.BusinessTypeName ?? "";
    }
}