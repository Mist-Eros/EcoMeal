using Ecomeal.EcoMealBlazor.Models;
using EcoMeal.EcoMealBlazor.Services;
using Microsoft.AspNetCore.Components;

namespace EcoMeal.EcoMealBlazor.Components.BusinessCard;

public partial class BusinessCard
{
    [Parameter]
    public required BusinessModel Business { get; set;}
    [Inject]
    public required BusinessService BusinessService { get; set;}
}