using System.ComponentModel.DataAnnotations;

namespace EcoMeal.EcoMealBlazor.Models;

public class PackageAddModel
{
    [Required(ErrorMessage = "Name is required")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "Description is required")]
    public string Description { get; set; } = string.Empty;

    [Required(ErrorMessage = "Price is required")]
    [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0")]
    public decimal Price { get; set; }

    [Required(ErrorMessage = "Pickup start is required")]
    public DateTime Start_PickUp { get; set; }

    [Required(ErrorMessage = "Pickup end is required")]
    public DateTime End_PickUp { get; set; }

    [Required(ErrorMessage = "Package type is required")]
    public int? PackageTypeId { get; set; }
}