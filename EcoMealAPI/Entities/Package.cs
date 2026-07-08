using System.ComponentModel.DataAnnotations.Schema;

namespace EcoMeal.EcoMealAPI.Entities;

public class Package
{
    public int Id { get; set; }
    public required string Name { get; set; }

    [ForeignKey(nameof(Business))]
    public required int BusinessId { get; set; }
    
    [ForeignKey(nameof(PackageType))]
    public required int PackageTypeId { get; set; }
    
    public string? Description { get; set; }
    public required decimal Price { get; set; }
    public required DateTime Start_PickUp { get; set; }
    public required DateTime End_PickUp { get; set; }
    
    public Business Business { get; set; }
    public PackageType PackageType { get; set; }
    public ICollection<Order> Orders { get; set; } = new List<Order>();
}