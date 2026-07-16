using EcoMeal.EcoMealBlazor.Models;
using EcoMeal.EcoMealBlazor.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using EcoMeal.EcoMealBlazor.Resources;

namespace EcoMeal.Components.Pages;

public class AddPackageBase : ComponentBase
{
    [Inject]
    protected NavigationManager NavigationManager { get; set; } = default!;
    [Inject]
    protected BusinessService BusinessService { get; set; } = default!;
    [Inject]
    protected IStringLocalizer<SharedResource> Loc { get; set; } = default!;


    [Parameter]
    public int BusinessId { get; set; }

    protected PackageAddModel Model = new();
    protected List<PackageTypeModel> PackageTypes = new();
    protected bool IsSubmitting = false;
    protected string? ErrorMessage;

    protected DateOnly? _pickupDate = DateOnly.FromDateTime(DateTime.Now);
    protected TimeOnly? _startTime = TimeOnly.FromDateTime(DateTime.Now);
    protected TimeOnly? _endTime = TimeOnly.FromDateTime(DateTime.Now.AddHours(2));

    protected override async Task OnInitializedAsync()
    {
        PackageTypes = await BusinessService.GetPackageTypesAsync();
    }

    protected void ApplyDateTime()
    {
        if (_pickupDate.HasValue && _startTime.HasValue && _endTime.HasValue)
        {
            Model.Start_PickUp = _pickupDate.Value.ToDateTime(_startTime.Value);
            Model.End_PickUp = _pickupDate.Value.ToDateTime(_endTime.Value);
        }
    }

    protected async Task HandleSubmit()
    {
        IsSubmitting = true;
        ErrorMessage = null;
        ApplyDateTime();

        var success = await BusinessService.AddPackageToBusinessAsync(BusinessId, Model);
        if (success)
        {
            NavigationManager.NavigateTo($"/business/{BusinessId}");
        }
        else
        {
            ErrorMessage = Loc["Something went wrong"].Value;
            IsSubmitting = false;
        }
    }

    protected void GoBack()
    {
        NavigationManager.NavigateTo($"/business/{BusinessId}");
    }
}
