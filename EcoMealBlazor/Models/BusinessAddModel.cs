using System.ComponentModel.DataAnnotations;

namespace EcoMeal.EcoMealBlazor.Models;

public class BusinessAddModel
{
    [Required(ErrorMessage = "Name is required")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "Address is required")]
    public string Address { get; set; } = string.Empty;

    public string? Description { get; set; }

    [Required(ErrorMessage = "Contact is required")]
    public string Contact { get; set; } = string.Empty;

    [Required(ErrorMessage = "Select a business type")]
    public int BusinessTypeId { get; set; }
}