using System;
using System.Collections.Generic;

namespace EcoMeal.EcoMealBlazor.Models;

public class BusinessDetailsModel : BusinessModel
{
    public List<PackageGetModel> Packages { get; set; } = new();
}