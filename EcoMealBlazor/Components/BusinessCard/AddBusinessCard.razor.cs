using Microsoft.AspNetCore.Components;

namespace EcoMeal.EcoMealBlazor.Components.BusinessCard;

public partial class AddBusinessCard
{
    [Parameter]
    public EventCallback OnAdd { get; set; }
}