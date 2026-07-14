using System.ComponentModel.DataAnnotations.Schema;

namespace EcoMeal.EcoMealAPI.Entities;

public class Business
{
    public int Id { get; set;}
    public required string Name { get; set;}
    public required string Address { get; set;}
    public string? Description { get; set;}
    public required string Contact { get; set;}
    
    [ForeignKey(nameof(BusinessType))]
    public int BusinessTypeId { get; set;}
    
    public BusinessType BusinessType { get; set;}
    
    public ICollection<Package> Packages { get; set; } = new List<Package>();
    public ICollection<Rating> Ratings { get; set; } = new List<Rating>();
}