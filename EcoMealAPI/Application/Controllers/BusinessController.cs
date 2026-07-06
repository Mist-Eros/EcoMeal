namespace EcoMeal.EcoMealAPI.Application.Controllers;
using EcoMeal.EcoMealAPI.Application;
using EcoMeal.EcoMealAPI.Entities;
using EcoMeal.EcoMealAPI.Infrastructure;
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
}