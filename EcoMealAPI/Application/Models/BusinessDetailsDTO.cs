using EcoMeal.EcoMealAPI.Entities;

namespace EcoMeal.EcoMealAPI.Models;

public class BusinessDetailsDTO : BusinessDTO
{
    public List<PackageDTO> Packages { get; set; } = new();
}