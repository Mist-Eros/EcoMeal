namespace EcoMeal.EcoMealAPI.Application.Controllers;
using EcoMeal.EcoMealAPI.Application;
using EcoMeal.EcoMealAPI.Entities;
using EcoMeal.EcoMealAPI.Infrastructure;
using EcoMeal.EcoMealAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("api/[controller]")]
public class BusinessController : ControllerBase
{
    // to remember, convention says private variables have _ at the start
    private readonly EcoMealDBContext _context;

    public BusinessController(EcoMealDBContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<BusinessDTO>>> GetAll()
    {
        var businessDTOs = await _context.Businesses
            .Include(b => b.BusinessType)
            .Select(b => new BusinessDTO
            {
                Id = b.Id,
                Name = b.Name,
                Address = b.Address,
                Description = b.Description,
                Contact = b.Contact,
                BusinessTypeName = b.BusinessType.Name
            })
            .ToListAsync();

        return Ok(businessDTOs);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(int id)
    {
        var business = await _context.Businesses.FindAsync(id);
        if(business is null)
        {
            return NotFound();
        }

        _context.Businesses.Remove(business);
        await _context.SaveChangesAsync();

        return NoContent();
    }
    [HttpGet("{id}")]
    public async Task<ActionResult<BusinessDetailsDTO>> GetOneById(int id)
    {
        var business = await _context.Businesses
            .Include(b => b.Packages)
            .ThenInclude(p => p.PackageType)
            .Select(b => new BusinessDetailsDTO
            {
                Id = b.Id,
                Name = b.Name,
                Address = b.Address,
                Description = b.Description,
                Contact = b.Contact,
                BusinessTypeName = b.BusinessType.Name,
                Packages = b.Packages.Select(p => new PackageDTO
                {
                    Id = p.Id,
                    Name = p.Name,
                    Description = p.Description,
                    Price = p.Price,
                    Start_PickUp = p.Start_PickUp,
                    End_PickUp = p.End_PickUp,
                    PackageTypeId = p.PackageTypeId,
                    PackageTypeName = p.PackageType.Name
                }).ToList()
            })
            .FirstOrDefaultAsync(b => b.Id == id);
            
        if (business is null)
        {
            return NotFound();
        }

        return Ok(business);
    }

    [HttpGet("types")]
    public async Task<ActionResult<IEnumerable<BusinessTypeDTO>>> GetBusinessTypes()
    {
        var types = await _context.BusinessType
            .Select(bt => new BusinessTypeDTO
            {
                Id = bt.Id,
                Name = bt.Name
            })
            .ToListAsync();
        
        return Ok(types);
    }

    [HttpPost]
    public async Task<ActionResult<BusinessDTO>> CreateBusiness([FromBody] BusinessAddDTO businessDto)
    {
        var business = new Business
        {
            Name = businessDto.Name,
            Address = businessDto.Address,
            Description = businessDto.Description,
            Contact = businessDto.Contact,
            BusinessTypeId = businessDto.BusinessTypeId,
            BusinessType = await _context.BusinessType.FindAsync(businessDto.BusinessTypeId)
        };

        _context.Businesses.Add(business);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetOneById), new { id = business.Id }, business);
    }
}