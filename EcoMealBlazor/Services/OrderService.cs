using System.Net.Http.Headers;
using EcoMeal.EcoMealBlazor.Models;

namespace EcoMeal.EcoMealBlazor.Services;

public class OrderService
{
    private readonly HttpClient _http;
    private readonly AuthService _authService;

    public OrderService(HttpClient http, AuthService authService)
    {
        _http = http;
        _authService = authService;
    }

    public async Task<bool> PlaceOrderAsync(int packageId)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, "api/order")
        {
            Content = JsonContent.Create(new { PackageId = packageId })
        };
        await AddAuthHeaderAsync(request);

        var response = await _http.SendAsync(request);
        return response.IsSuccessStatusCode;
    }

    public async Task<List<OrderGetModel>> GetMyOrderAsync()
    {
        var request = new HttpRequestMessage(HttpMethod.Get, "api/order/my");
        await AddAuthHeaderAsync(request);

        var response = await _http.SendAsync(request);
        response.EnsureSuccessStatusCode();

        var orders = await response.Content.ReadFromJsonAsync<List<OrderGetModel>>();
        return orders ?? new List<OrderGetModel>();
    }


    public async Task<bool> ClearOrderHistoryAsync()
    {
        var request = new HttpRequestMessage(HttpMethod.Delete, "api/order");
        await AddAuthHeaderAsync(request);

        var response = await _http.SendAsync(request);
        return response.IsSuccessStatusCode;
    }

    private async Task AddAuthHeaderAsync(HttpRequestMessage request)
    {
        if (string.IsNullOrEmpty(_authService.Token))
        {
            await _authService.LoadTokenAsync();
        }

        if (!string.IsNullOrEmpty(_authService.Token))
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _authService.Token);
        }
    }
}