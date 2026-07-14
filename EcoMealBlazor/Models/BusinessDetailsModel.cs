using System;
using System.Collections.Generic;

namespace EcoMeal.EcoMealBlazor.Models;

public class BusinessDetailsModel : BusinessModel
{
    public int BusinessTypeId { get; set; }
    public List<PackageGetModel> Packages { get; set; } = new();
    public double AverageRating { get; set; }
    public int TotalRatings { get; set; }
    public int? UserRating { get; set; }
}