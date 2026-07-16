using EcoMeal.EcoMealBlazor.Models;
using EcoMeal.EcoMealBlazor.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using EcoMeal.EcoMealBlazor.Resources;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EcoMeal.Components.Pages;

public class LastChanceBase : ComponentBase
{
    [Inject]
    protected BusinessService BusinessService { get; set; } = default!;
    [Inject]
    protected NavigationManager NavigationManager { get; set; } = default!;
    [Inject]
    protected IStringLocalizer<SharedResource> Loc { get; set; } = default!;
    [Inject]
    protected CurrencyService Currency { get; set; } = default!;


    protected List<ExpiringPackageModel>? _packages;

    protected override async Task OnInitializedAsync()
    {
        _packages = await BusinessService.GetExpiringPackagesAsync();
    }

    protected void GoToBusiness(int businessId)
    {
        NavigationManager.NavigateTo($"/business/{businessId}");
    }

    protected void GoBack()
    {
        NavigationManager.NavigateTo("/businesses");
    }

    protected string GetTimeRemaining(DateTime endPickup)
    {
        var remaining = endPickup - DateTime.Now;
        if (remaining.TotalMinutes <= 0) return "Expired";
        if (remaining.TotalHours < 1) return $"{(int)remaining.TotalMinutes}m left";
        if (remaining.TotalHours < 24) return $"{(int)remaining.TotalHours}h {(int)remaining.Minutes}m left";
        return $"{(int)remaining.TotalDays}d left";
    }
}
