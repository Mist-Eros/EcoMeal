using EcoMeal.EcoMealAPI.Entities;

namespace EcoMeal.EcoMealAPI.Models;

public class BusinessDetailsDTO : BusinessDTO
{
    public int BusinessTypeId { get; set; }
    public List<PackageDTO> Packages { get; set; } = new();
    public double AverageRating { get; set; }
    public int TotalRatings { get; set; }
    public int? UserRating { get; set; }
}