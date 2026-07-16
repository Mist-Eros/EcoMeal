using EcoMeal.EcoMealBlazor.Models;
using EcoMeal.EcoMealBlazor.Services;
using Microsoft.Extensions.Localization;
using EcoMeal.EcoMealBlazor.Resources;
using Microsoft.AspNetCore.Components;

namespace EcoMeal.Components;

public class SearchBarBase : ComponentBase
{
    [Parameter]
    public EventCallback<SearchCriteria> OnSearch { get; set; }

    protected string SearchTerm = "";
    protected string SelectedType = "";
    protected string SearchBy = "name";
    protected bool _searchTermVisible = false;
    protected List<BusinessTypeModel> BusinessTypes = new();

    [Inject]
    protected BusinessService BusinessService { get; set; } = default!;
    [Inject]
    protected NavigationManager NavigationManager { get; set; } = default!;
    [Inject]
    protected IStringLocalizer<SharedResource> Loc { get; set; } = default!;

    protected override async Task OnInitializedAsync()
    {
        BusinessTypes = await BusinessService.GetBusinessTypesAsync();
    }

    protected async Task SetTypeAll() => await SetType("");
    protected async Task SetTypeItem(string type) => await SetType(type);

    protected async Task SetType(string type)
    {
        SelectedType = type;
        await ApplySearch();
    }

    protected async Task SetSearchByName() => await SetSearchBy("name");
    protected async Task SetSearchByAddress() => await SetSearchBy("address");

    protected async Task SetSearchBy(string by)
    {
        SearchBy = by;
        await ApplySearch();
    }

    protected async Task OnSearchTermChanged()
    {
        _searchTermVisible = !string.IsNullOrEmpty(SearchTerm);
        await ApplySearch();
    }

    protected async Task ClearSearch()
    {
        SearchTerm = "";
        _searchTermVisible = false;
        await ApplySearch();
    }

    protected async Task ApplySearch()
    {
        var criteria = new SearchCriteria
        {
            Term = SearchTerm,
            Type = SelectedType,
            SearchBy = SearchBy
        };
        await OnSearch.InvokeAsync(criteria);
    }

    protected string GetTypeIcon(string typeName)
    {
        var t = typeName?.ToLower() ?? "";
        if (t.Contains("restaurant")) return "fa-solid fa-pizza-slice";
        if (t.Contains("fast food") || t.Contains("fast-food")) return "fa-solid fa-burger";
        if (t.Contains("bakery")) return "fa-solid fa-cake-candles";
        return "fa-solid fa-shop";
    }

    protected string LocTypeName(string typeName)
    {
        var t = typeName?.ToLower() ?? "";
        if (t.Contains("restaurant")) return Loc["Restaurant"];
        if (t.Contains("fast food") || t.Contains("fast-food")) return Loc["Fast Food"];
        if (t.Contains("bakery")) return Loc["Bakery"];
        return typeName;
    }

    public class SearchCriteria
    {
        public string Term { get; set; } = "";
        public string Type { get; set; } = "";
        public string SearchBy { get; set; } = "name";
    }
}
