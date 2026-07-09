using EcoMeal.EcoMealBlazor.Models;
using EcoMeal.EcoMealBlazor.Services;
using Microsoft.AspNetCore.Components;

namespace EcoMeal.EcoMealBlazor.Components.BusinessList;

public partial class BusinessList
{
    [Inject]
    public required BusinessService BusinessService { get; set; }

    private List<BusinessModel>? AllBusinesses;
    private List<BusinessModel>? FilteredBusinesses;

    protected override async Task OnInitializedAsync()
    {
        await LoadBusinesses();
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
}