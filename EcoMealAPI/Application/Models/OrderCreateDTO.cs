using System.ComponentModel.DataAnnotations;

namespace EcoMeal.EcoMealAPI.Models;

public class OrderCreateDTO
{
    [Required]
    public int PackageId { get; set;}
}