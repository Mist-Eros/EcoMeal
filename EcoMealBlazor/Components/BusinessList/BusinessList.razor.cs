using EcoMeal.EcoMealBlazor.Models;
using EcoMeal.EcoMealBlazor.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;

namespace EcoMeal.EcoMealBlazor.Components.BusinessList;

public partial class BusinessList
{
    [Inject]
    public required BusinessService BusinessService { get; set; }
    
    [Inject]
    public required AuthenticationStateProvider AuthStateProvider { get; set; }
    
    [Inject]
    public required NavigationManager NavigationManager { get; set; }

    private List<BusinessModel>? AllBusinesses;
    private List<BusinessModel>? FilteredBusinesses;
    private bool _isAdmin = false;

    protected override async Task OnInitializedAsync()
    {
        await LoadBusinesses();
        
        var authState = await AuthStateProvider.GetAuthenticationStateAsync();
        var user = authState.User;
        _isAdmin = user.IsInRole("Admin");
    }

    private async Task LoadBusinesses()
    {
        AllBusinesses = await BusinessService.GetAllAsync();
        FilteredBusinesses = AllBusinesses;
        StateHasChanged();
    }

    private async Task HandleSearch(SearchBar.SearchCriteria criteria)
    {
        if (AllBusinesses == null) return;

        var query = AllBusinesses.AsQueryable();

        if (!string.IsNullOrEmpty(criteria.Type))
        {
            query = query.Where(b => b.BusinessTypeName == criteria.Type);
        }

        if (!string.IsNullOrEmpty(criteria.Term))
        {
            if (criteria.SearchBy == "name")
            {
                query = query.Where(b => b.Name.Contains(criteria.Term, StringComparison.OrdinalIgnoreCase));
            }
            else if (criteria.SearchBy == "address")
            {
                query = query.Where(b => b.Address.Contains(criteria.Term, StringComparison.OrdinalIgnoreCase));
            }
        }

        FilteredBusinesses = query.ToList();
        StateHasChanged();
    }

    private async Task RefreshList()
    {
        await LoadBusinesses();
    }

    private void NavigateToAddBusiness()
    {
        NavigationManager.NavigateTo("/business/create");
    }
}
