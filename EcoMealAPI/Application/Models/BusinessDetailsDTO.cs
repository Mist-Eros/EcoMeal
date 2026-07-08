using EcoMeal.EcoMealAPI.Entities;

namespace EcoMeal.EcoMealAPI.Models;

public class BusinessDetailsDTO : BusinessDTO
{
    public int BusinessTypeId { get; set; }
    public List<PackageDTO> Packages { get; set; } = new();
}