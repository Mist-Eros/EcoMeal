using Microsoft.AspNetCore.Identity;

namespace EcoMeal.EcoMealAPI.Entities;

public class User : IdentityUser<int>
{
    public string? Name { get; set;}
    public string? Contact { get; set;}
}