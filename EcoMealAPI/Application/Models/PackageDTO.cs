namespace EcoMeal.EcoMealAPI.Models;

public class PackageDTO
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public DateTime Start_PickUp { get; set; }
    public DateTime End_PickUp { get; set; }
    public int PackageTypeId { get; set; }
    public string? PackageTypeName { get; set; }
}