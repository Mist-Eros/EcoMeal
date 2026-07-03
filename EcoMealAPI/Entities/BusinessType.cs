using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace EcoMeal.EcoMealAPI.Entities;

public class BusinessType
{
    [Key]
    public int Id { get; set;}
    [MaxLength(20)]
    public required string Name { get; set;}
    
}