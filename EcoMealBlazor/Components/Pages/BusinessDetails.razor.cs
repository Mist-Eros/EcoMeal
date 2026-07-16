using EcoMeal.EcoMealBlazor.Models;
using EcoMeal.EcoMealBlazor.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Localization;
using EcoMeal.EcoMealBlazor.Resources;
using Microsoft.JSInterop;
using System;
using System.Threading.Tasks;

namespace EcoMeal.Components.Pages;

public class BusinessDetailsBase : ComponentBase
{
    [Inject]
    protected BusinessService BusinessService { get; set; } = default!;
    [Inject]
    protected NavigationManager NavigationManager { get; set; } = default!;
    [Inject]
    protected IJSRuntime JSRuntime { get; set; } = default!;
    [Inject]
    protected AuthenticationStateProvider AuthStateProvider { get; set; } = default!;
    [Inject]
    protected IStringLocalizer<SharedResource> Loc { get; set; } = default!;
    [Inject]
    protected CurrencyService Currency { get; set; } = default!;


    [Parameter]
    public int Id { get; set; }

    protected BusinessDetailsModel? Business;
    protected bool _isAdmin = false;

    protected bool _showDeleteConfirm = false;
    protected string _confirmMessage = "";
    protected int _deletePackageId = 0;
    protected bool _showQrModal = false;

    protected override async Task OnInitializedAsync()
    {
        Business = await BusinessService.GetOneById(Id);
        
        var authState = await AuthStateProvider.GetAuthenticationStateAsync();
        var user = authState.User;
        _isAdmin = user.IsInRole("Admin");
    }

    protected string GetQRUrl()
    {
        var url = NavigationManager.Uri;
        return $"https://api.qrserver.com/v1/create-qr-code/?size=180x180&data={Uri.EscapeDataString(url)}";
    }

    protected string GetBusinessImage()
    {
        var type = Business?.BusinessTypeName?.ToLower() ?? "";
        
        if (type.Contains("restaurant"))
            return "https://images.unsplash.com/photo-1517248135467-4c7edcad34c4?w=800&h=400&fit=crop";
        else if (type.Contains("fast food") || type.Contains("fast-food"))
            return "https://images.unsplash.com/photo-1571091718767-18b5b1457add?w=800&h=400&fit=crop";
        else if (type.Contains("bakery"))
            return "https://images.unsplash.com/photo-1555507036-ab1f4038808a?w=800&h=400&fit=crop";
        else
            return "https://images.unsplash.com/photo-1555396273-367ea4eb4db5?w=800&h=400&fit=crop";
    }

    protected string GetPackageImage(string packageType)
    {
        var type = packageType?.ToLower() ?? "";
        
        if (type.Contains("meal") || type.Contains("combo"))
            return "https://images.unsplash.com/photo-1546069901-ba9599a7e63c?w=400&h=300&fit=crop";
        else if (type.Contains("snack") || type.Contains("appetizer"))
            return "https://images.unsplash.com/photo-1565299624946-b28f40a0ae38?w=400&h=300&fit=crop";
        else if (type.Contains("drink") || type.Contains("beverage"))
            return "https://images.unsplash.com/photo-1544145945-f90425340c7e?w=400&h=300&fit=crop";
        else if (type.Contains("dessert"))
            return "https://images.unsplash.com/photo-1563805042-7684c019e1cb?w=400&h=300&fit=crop";
        else
            return "https://images.unsplash.com/photo-1546069901-ba9599a7e63c?w=400&h=300&fit=crop";
    }

    protected void NavigateToAddPackage()
    {
        NavigationManager.NavigateTo($"/business/{Id}/addpackage");
    }

    protected void NavigateToEditPackage(int packageId)
    {
        NavigationManager.NavigateTo($"/business/{Id}/package/{packageId}/edit");
    }

    protected void DeletePackage(int packageId)
    {
        _deletePackageId = packageId;
        _confirmMessage = Loc["Confirm delete package"].Value;
        _showDeleteConfirm = true;
    }

    protected async Task ExecuteDeletePackage()
    {
        _showDeleteConfirm = false;
        var success = await BusinessService.DeletePackageAsync(_deletePackageId);
        if (success)
        {
            Business = await BusinessService.GetOneById(Id);
            StateHasChanged();
        }
    }

    protected void OrderPackage(int packageId)
    {
        NavigationManager.NavigateTo($"/business/{Id}/package/{packageId}/order");
    }

    protected void GoBack()
    {
        NavigationManager.NavigateTo("/businesses/");
    }

    protected string GetTypeIcon()
    {
        var type = Business?.BusinessTypeName?.ToLower() ?? "";
        if (type.Contains("restaurant")) return "fa-solid fa-pizza-slice";
        if (type.Contains("fast food") || type.Contains("fast-food")) return "fa-solid fa-burger";
        if (type.Contains("bakery")) return "fa-solid fa-cake-candles";
        return "fa-solid fa-shop";
    }

    protected string LocTypeName()
    {
        var t = Business?.BusinessTypeName?.ToLower() ?? "";
        if (t.Contains("restaurant")) return Loc["Restaurant"];
        if (t.Contains("fast food") || t.Contains("fast-food")) return Loc["Fast Food"];
        if (t.Contains("bakery")) return Loc["Bakery"];
        return Business?.BusinessTypeName ?? "";
    }
}
