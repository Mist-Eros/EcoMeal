using EcoMeal.EcoMealAPI.Entities;
using EcoMeal.EcoMealAPI.Infrastructure;
using EcoMeal.EcoMealAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EcoMeal.EcoMealAPI.Application.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PackageTypesController : ControllerBase
{
    private readonly EcoMealDBContext _context;

    public PackageTypesController(EcoMealDBContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<PackageTypeDTO>>> GetPackageTypes()
    {
        var types = await _context.PackageType
            .Select(pt => new PackageTypeDTO
            {
                Id = pt.Id,
                Name = pt.Name
            })
            .ToListAsync();

        return Ok(types);
    }
}