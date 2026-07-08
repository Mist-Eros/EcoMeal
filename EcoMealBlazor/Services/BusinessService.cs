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
    public async Task<List<PackageTypeModel>> GetPackageTypesAsync()
    {
        try
        {
            var types = await _http.GetFromJsonAsync<List<PackageTypeModel>>("api/PackageTypes");
            return types ?? new List<PackageTypeModel>();
        }
        catch
        {
            return new List<PackageTypeModel>();
        }
    }

    public async Task<bool> AddPackageToBusinessAsync(int businessId, PackageAddModel model)
    {
        try
        {
            var response = await _http.PostAsJsonAsync($"api/business/{businessId}/addPackage", model);
            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> DeletePackageAsync(int packageId)
    {
        try
        {
            var response = await _http.DeleteAsync($"api/package/{packageId}");
            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> UpdateBusinessAsync(BusinessEditModel model)
    {
        try
        {
            var response = await _http.PutAsJsonAsync($"api/business/{model.Id}", model);
            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }
    public async Task<PackageGetModel?> GetPackageByIdAsync(int packageId)
    {
        try
        {
            var package = await _http.GetFromJsonAsync<PackageGetModel>($"api/package/{packageId}");
            return package;
        }
        catch
        {
            return null;
        }
    }

    public async Task<bool> UpdatePackageAsync(PackageEditModel model)
    {
        try
        {
            var response = await _http.PutAsJsonAsync($"api/package/{model.Id}", model);
            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }
}