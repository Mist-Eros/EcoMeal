using EcoMeal.EcoMealBlazor.Models;
using EcoMeal.EcoMealBlazor.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using EcoMeal.EcoMealBlazor.Resources;

namespace EcoMeal.Components.Pages;

public class EditBusinessBase : ComponentBase
{
    [Inject]
    protected NavigationManager NavigationManager { get; set; } = default!;
    [Inject]
    protected BusinessService BusinessService { get; set; } = default!;
    [Inject]
    protected IStringLocalizer<SharedResource> Loc { get; set; } = default!;


    [Parameter]
    public int Id { get; set; }

    protected BusinessEditModel EditModel = new();
    protected List<BusinessTypeModel> BusinessTypes = new();
    protected bool IsSubmitting = false;
    protected string? ErrorMessage;

    protected override async Task OnInitializedAsync()
    {
        var business = await BusinessService.GetOneById(Id);
        if (business != null)
        {
            EditModel = new BusinessEditModel
            {
                Id = business.Id,
                Name = business.Name,
                Address = business.Address,
                Description = business.Description,
                Contact = business.Contact,
                BusinessTypeId = business.BusinessTypeId
            };
        }
        BusinessTypes = await BusinessService.GetBusinessTypesAsync();
    }

    protected async Task HandleSubmit()
    {
        IsSubmitting = true;
        ErrorMessage = null;

        var success = await BusinessService.UpdateBusinessAsync(EditModel);
        if (success)
        {
            NavigationManager.NavigateTo("/businesses/");
        }
        else
        {
            ErrorMessage = Loc["Something went wrong"].Value;
            IsSubmitting = false;
        }
    }

    protected void GoBack()
    {
        NavigationManager.NavigateTo("/businesses/");
    }
}
