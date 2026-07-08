using EcoMeal.EcoMealBlazor.Models;

namespace EcoMeal.EcoMealBlazor.Services;

public class BusinessService
{
    private readonly HttpClient _http;

    public BusinessService(HttpClient http)
    {
        _http = http;
    }

    public async Task<List<BusinessModel>> GetAllAsync()
    {
        var businesses = await _http.GetFromJsonAsync<List<BusinessModel>>("api/business");
        return businesses ?? new List<BusinessModel>();
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var response = await _http.DeleteAsync($"api/business/{id}");
        return response.IsSuccessStatusCode;
    }

    public async Task<BusinessDetailsModel?> GetOneById(int Id)
    {
        var business = await _http.GetFromJsonAsync<BusinessDetailsModel>($"api/business/{Id}");
        return business;
    }
    public async Task<List<BusinessTypeModel>> GetBusinessTypesAsync()
    {
        try
        {
            var types = await _http.GetFromJsonAsync<List<BusinessTypeModel>>("api/business/types");
            return types ?? new List<BusinessTypeModel>();
        }
        catch
        {
            return new List<BusinessTypeModel>();
        }
    }

    public async Task<bool> CreateBusinessAsync(BusinessAddModel model)
    {
        try
        {
            var response = await _http.PostAsJsonAsync("api/business", model);
            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }
}