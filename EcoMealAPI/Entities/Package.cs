using System.ComponentModel.DataAnnotations.Schema;

namespace EcoMeal.EcoMealAPI.Entities;

public class Package
{
    public int Id { get; set; }
    public required int No_Package { get; set; }
    
    [ForeignKey(nameof(Business))]
    public required int BusinessId { get; set; }
    
    [ForeignKey(nameof(PackageType))]
    public required int PackageTypeId { get; set; }
    
    public string? Description { get; set; }
    public required decimal Price { get; set; }  // ← Make sure this exists
    public required DateTime Start_PickUp { get; set; }
    public required DateTime End_Pickup { get; set; }
    
    // Navigation properties
    public Business Business { get; set; }
    public PackageType PackageType { get; set; }
    public ICollection<Order> Orders { get; set; } = new List<Order>();
}