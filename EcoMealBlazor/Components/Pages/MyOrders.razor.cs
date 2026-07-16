using EcoMeal.EcoMealBlazor.Models;
using EcoMeal.EcoMealBlazor.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Localization;
using EcoMeal.EcoMealBlazor.Resources;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EcoMeal.Components.Pages;

public class MyOrdersBase : ComponentBase
{
    [Inject]
    protected OrderService OrderService { get; set; } = default!;
    [Inject]
    protected NavigationManager NavigationManager { get; set; } = default!;
    [Inject]
    protected AuthenticationStateProvider AuthStateProvider { get; set; } = default!;
    [Inject]
    protected IJSRuntime JSRuntime { get; set; } = default!;
    [Inject]
    protected BusinessService BusinessService { get; set; } = default!;
    [Inject]
    protected IStringLocalizer<SharedResource> Loc { get; set; } = default!;
    [Inject]
    protected CurrencyService Currency { get; set; } = default!;


    protected string _activeTab = "orders";
    protected bool _isLoading = true;
    protected bool _isAdmin = false;
    protected List<OrderGetModel> _orders = new();
    protected int _orderCount = 0;
    protected int _discountPercent = 0;
    protected double _totalPrice = 0;
    protected double _savedAmount = 0;
    protected double _finalPrice = 0;

    protected bool _showRating = false;
    protected List<(int BusinessId, string Name)> _ratingBusinesses = new();
    protected int _ratingIndex = 0;
    protected string _ratingBusinessName = "";
    protected int _ratingValue = 0;
    protected int _ratingHover = 0;
    protected bool _ratingSubmitting = false;
    protected string _ratingError = "";

    protected bool _showCancelConfirm = false;
    protected string _confirmMessage = "";
    protected int _cancelOrderId = 0;

    protected override async Task OnInitializedAsync()
    {
        var authState = await AuthStateProvider.GetAuthenticationStateAsync();
        _isAdmin = authState.User.IsInRole("Admin");

        await LoadOrders();
        _isLoading = false;
    }

    protected async Task LoadOrders()
    {
        if (_activeTab == "orders")
        {
            _orders = _isAdmin
                ? await OrderService.GetAllOrdersAsync()
                : await OrderService.GetMyOrderAsync();

            var pending = _orders.Where(o => o.Status == 1).ToList();
            _orderCount = pending.Count;
            _discountPercent = pending.Count * 5;
            _totalPrice = pending.Sum(o => o.Price);
            _savedAmount = _totalPrice * _discountPercent / 100.0;
            _finalPrice = _totalPrice - _savedAmount;
        }
        else
        {
            _orders = await OrderService.GetOrderHistoryAsync();
            _discountPercent = 0;
            _orderCount = 0;
        }
    }

    protected async Task SwitchToOrders() => await SwitchTab("orders");
    protected async Task SwitchToHistory() => await SwitchTab("history");

    protected async Task SwitchTab(string tab)
    {
        _activeTab = tab;
        _isLoading = true;
        StateHasChanged();

        await LoadOrders();
        _isLoading = false;
    }

    protected async Task HandlePickup(int orderId, int businessId, string businessName)
    {
        var success = await OrderService.MarkPickedUpAsync(orderId);
        if (success)
        {
            await LoadOrders();
            StartRating(new List<(int, string)> { (businessId, businessName) });
        }
    }

    protected async Task HandleCancel(int orderId)
    {
        _cancelOrderId = orderId;
        _confirmMessage = Loc["Confirm cancel"].ToString();
        _showCancelConfirm = true;
    }

    protected async Task ConfirmCancel()
    {
        _showCancelConfirm = false;
        var success = await OrderService.CancelOrderAsync(_cancelOrderId);
        if (success)
        {
            await LoadOrders();
            StateHasChanged();
        }
    }

    protected async Task HandlePickUpAll()
    {
        var pending = _orders.Where(o => o.Status == 1).ToList();
        var uniqueBusinesses = pending
            .Select(o => (o.businessId, o.BusinessName))
            .Distinct()
            .ToList();

        var success = await OrderService.MarkAllPickedUpAsync();
        if (success)
        {
            await LoadOrders();
            StartRating(uniqueBusinesses);
        }
    }

    protected void StartRating(List<(int BusinessId, string Name)> businesses)
    {
        if (businesses.Count == 0)
        {
            _showRating = false;
            StateHasChanged();
            return;
        }

        _ratingBusinesses = businesses;
        _ratingIndex = 0;
        _ratingValue = 0;
        _ratingHover = 0;
        _ratingBusinessName = businesses[0].Name;
        _showRating = true;
        StateHasChanged();
    }

    protected async Task SubmitRating(int stars)
    {
        _ratingValue = stars;
        _ratingHover = 0;
    }

    protected void HoverStar(int star) { _ratingHover = star; }
    protected void ResetHover() { _ratingHover = 0; }

    protected async Task ConfirmRating()
    {
        if (_ratingValue == 0) return;

        _ratingSubmitting = true;
        _ratingError = "";
        StateHasChanged();

        var (businessId, _) = _ratingBusinesses[_ratingIndex];
        var success = await BusinessService.RateBusinessAsync(businessId, _ratingValue);

        if (!success)
        {
            _ratingError = Loc["Failed to save rating"].ToString();
            _ratingSubmitting = false;
            StateHasChanged();
            return;
        }

        if (_ratingIndex < _ratingBusinesses.Count - 1)
        {
            _ratingIndex++;
            _ratingValue = 0;
            _ratingHover = 0;
            _ratingBusinessName = _ratingBusinesses[_ratingIndex].Name;
            _ratingSubmitting = false;
        }
        else
        {
            _showRating = false;
        }

        StateHasChanged();
    }

    protected string StatusLabel(int status) => status switch
    {
        1 => "Placed",
        2 => "Picked Up",
        0 => "Cancelled",
        _ => "Unknown"
    };

    protected string StatusClass(int status) => status switch
    {
        1 => "status-placed",
        2 => "status-picked",
        0 => "status-cancelled",
        _ => ""
    };

    protected void GoToBusinesses()
    {
        NavigationManager.NavigateTo("/businesses");
    }
}
