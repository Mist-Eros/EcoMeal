namespace EcoMeal.EcoMealBlazor.Models;

public class ExpiringPackageModel
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public DateTime Start_PickUp { get; set; }
    public DateTime End_PickUp { get; set; }
    public int PackageTypeId { get; set; }
    public string? PackageTypeName { get; set; }
    public int BusinessId { get; set; }
    public string BusinessName { get; set; } = string.Empty;
    public string BusinessTypeName { get; set; } = string.Empty;
}
