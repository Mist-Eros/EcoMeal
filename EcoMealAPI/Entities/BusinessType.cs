using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EcoMeal.EcoMealAPI.Entities;

public class BusinessType
{
    [Key]
    public int Id { get; set;}
    
    [MaxLength(20)]
    public required string Name { get; set;}

    public ICollection<Business> Businesses { get; set; } = new List<Business>();
}