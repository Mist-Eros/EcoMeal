using EcoMeal.EcoMealAPI.Entities;
using EcoMeal.EcoMealAPI.Infrastructure;
using EcoMeal.EcoMealAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EcoMeal.EcoMealAPI.Application.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PackageController : ControllerBase
{
    private readonly EcoMealDBContext _context;

    public PackageController(EcoMealDBContext context)
    {
        _context = context;
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<PackageDTO>> GetPackageById(int id)
    {
        var package = await _context.Package
            .Include(p => p.PackageType)
            .Select(p => new PackageDTO
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                Price = p.Price,
                Start_PickUp = p.Start_PickUp,
                End_PickUp = p.End_PickUp,
                PackageTypeId = p.PackageTypeId,
                PackageTypeName = p.PackageType.Name
            })
            .FirstOrDefaultAsync(p => p.Id == id);

        if (package == null)
        {
            return NotFound();
        }

        return Ok(package);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdatePackage(int id, [FromBody] PackageEditDTO packageDto)
    {
        if (id != packageDto.Id)
        {
            return BadRequest();
        }

        var package = await _context.Package.FindAsync(id);
        if (package == null)
        {
            return NotFound();
        }

        package.Name = packageDto.Name;
        package.Description = packageDto.Description;
        package.Price = packageDto.Price;
        package.Start_PickUp = packageDto.Start_PickUp;
        package.End_PickUp = packageDto.End_PickUp;
        package.PackageTypeId = packageDto.PackageTypeId;

        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeletePackage(int id)
    {
        var package = await _context.Package.FindAsync(id);
        if (package == null)
        {
            return NotFound();
        }

        _context.Package.Remove(package);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}