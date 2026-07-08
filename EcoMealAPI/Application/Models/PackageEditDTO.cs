namespace EcoMeal.EcoMealAPI.Models;

public class PackageEditDTO
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required string Description { get; set; }
    public decimal Price { get; set; }
    public DateTime Start_PickUp { get; set; }
    public DateTime End_PickUp { get; set; }
    public int PackageTypeId { get; set; }
}