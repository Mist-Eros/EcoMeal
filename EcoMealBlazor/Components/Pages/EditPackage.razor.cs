using EcoMeal.EcoMealBlazor.Models;
using EcoMeal.EcoMealBlazor.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using EcoMeal.EcoMealBlazor.Resources;

namespace EcoMeal.Components.Pages;

public class EditPackageBase : ComponentBase
{
    [Inject]
    protected NavigationManager NavigationManager { get; set; } = default!;
    [Inject]
    protected BusinessService BusinessService { get; set; } = default!;
    [Inject]
    protected IStringLocalizer<SharedResource> Loc { get; set; } = default!;


    [Parameter]
    public int BusinessId { get; set; }

    [Parameter]
    public int PackageId { get; set; }

    protected PackageEditModel EditModel = new();
    protected List<PackageTypeModel> PackageTypes = new();
    protected bool IsSubmitting = false;
    protected string? ErrorMessage;
    protected bool _isLoading = true;

    protected DateOnly? _pickupDate;
    protected TimeOnly? _startTime;
    protected TimeOnly? _endTime;

    protected override async Task OnInitializedAsync()
    {
        var package = await BusinessService.GetPackageByIdAsync(PackageId);
        if (package != null)
        {
            EditModel = new PackageEditModel
            {
                Id = package.Id,
                Name = package.Name,
                Description = package.Description,
                Price = package.Price,
                Start_PickUp = package.Start_PickUp,
                End_PickUp = package.End_PickUp,
                PackageTypeId = package.PackageTypeId
            };
            _pickupDate = DateOnly.FromDateTime(package.Start_PickUp);
            _startTime = TimeOnly.FromDateTime(package.Start_PickUp);
            _endTime = TimeOnly.FromDateTime(package.End_PickUp);
        }
        PackageTypes = await BusinessService.GetPackageTypesAsync();
        _isLoading = false;
    }

    protected void ApplyDateTime()
    {
        if (_pickupDate.HasValue && _startTime.HasValue && _endTime.HasValue)
        {
            EditModel.Start_PickUp = _pickupDate.Value.ToDateTime(_startTime.Value);
            EditModel.End_PickUp = _pickupDate.Value.ToDateTime(_endTime.Value);
        }
    }

    protected async Task HandleSubmit()
    {
        IsSubmitting = true;
        ErrorMessage = null;
        ApplyDateTime();

        var success = await BusinessService.UpdatePackageAsync(EditModel);
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
