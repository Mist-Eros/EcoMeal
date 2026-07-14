namespace EcoMeal.EcoMealAPI.Application.Controllers;
using System.Security.Claims;
using EcoMeal.EcoMealAPI.Entities;
using EcoMeal.EcoMealAPI.Infrastructure;
using EcoMeal.EcoMealAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("api/[controller]")]
public class BusinessController : ControllerBase
{
    private readonly EcoMealDBContext _context;

    public BusinessController(EcoMealDBContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<BusinessDTO>>> GetAll()
    {
        var query = _context.Businesses
            .Include(b => b.BusinessType)
            .Select(b => new BusinessDTO
            {
                Id = b.Id,
                Name = b.Name,
                Address = b.Address,
                Description = b.Description,
                Contact = b.Contact,
                BusinessTypeName = b.BusinessType.Name,
                AverageRating = 0,
                TotalRatings = 0
            });

        var businessDTOs = await query.ToListAsync();

        try
        {
            var ratings = await _context.Ratings.ToListAsync();
            foreach (var b in businessDTOs)
            {
                var bizRatings = ratings.Where(r => r.BusinessId == b.Id).ToList();
                b.TotalRatings = bizRatings.Count;
                b.AverageRating = bizRatings.Count > 0 ? bizRatings.Average(r => r.Stars) : 0;
            }
        }
        catch { }

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
        var userIdValue = User.FindFirstValue(ClaimTypes.NameIdentifier);
        int? userId = userIdValue != null ? int.Parse(userIdValue) : null;

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
                BusinessTypeId = b.BusinessTypeId,
                Packages = b.Packages
                .Where (p => p.Orders.Count == 0)
                .Select (p => new PackageDTO
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
            return NotFound();

        try
        {
            var ratings = await _context.Ratings
                .Where(r => r.BusinessId == id)
                .ToListAsync();

            business.TotalRatings = ratings.Count;
            business.AverageRating = ratings.Count > 0 ? ratings.Average(r => r.Stars) : 0;
            business.UserRating = userId.HasValue
                ? ratings.FirstOrDefault(r => r.UserId == userId.Value)?.Stars
                : null;
        }
        catch
        {
            business.AverageRating = 0;
            business.TotalRatings = 0;
            business.UserRating = null;
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
            BusinessTypeId = businessDto.BusinessTypeId
        };

        _context.Businesses.Add(business);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetOneById), new { id = business.Id }, business);
    }

    [HttpPost("{id}/addPackage")]
    public async Task<IActionResult> AddPackageToBusiness(int id, [FromBody] PackageAddDTO packageDto)
    {
        var business = await _context.Businesses.FindAsync(id);
        if (business == null)
        {
            return NotFound($"Business with ID {id} not found");
        }

        var packageType = await _context.PackageType.FindAsync(packageDto.PackageTypeId);
        if (packageType == null)
        {
            return BadRequest($"Package type with ID {packageDto.PackageTypeId} not found");
        }

        var package = new Package
        {
            Name = packageDto.Name,
            Description = packageDto.Description,
            Price = packageDto.Price,
            Start_PickUp = packageDto.Start_PickUp,
            End_PickUp = packageDto.End_PickUp,
            PackageTypeId = packageDto.PackageTypeId,
            BusinessId = id
        };

        _context.Package.Add(package);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetOneById), new { id = id }, package);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateBusiness(int id, [FromBody] BusinessEditDTO businessDto)
    {
        if (id != businessDto.Id)
        {
            return BadRequest();
        }

        var business = await _context.Businesses.FindAsync(id);
        if (business == null)
        {
            return NotFound();
        }

        business.Name = businessDto.Name;
        business.Address = businessDto.Address;
        business.Description = businessDto.Description;
        business.Contact = businessDto.Contact;
        business.BusinessTypeId = businessDto.BusinessTypeId;

        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpPost("{businessId}/rate")]
    [Authorize]
    public async Task<ActionResult> RateBusiness(int businessId, [FromBody] RateRequest request)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        _context.Ratings.Add(new Rating
        {
            UserId = userId,
            BusinessId = businessId,
            Stars = request.Stars
        });

        await _context.SaveChangesAsync();
        return Ok(new { Message = "Rating submitted" });
    }

    [HttpGet("{businessId}/rating")]
    public async Task<ActionResult<object>> GetRating(int businessId)
    {
        var userIdValue = User.FindFirstValue(ClaimTypes.NameIdentifier);
        int? userId = userIdValue != null ? int.Parse(userIdValue) : null;

        var ratings = await _context.Ratings
            .Where(r => r.BusinessId == businessId)
            .ToListAsync();

        var avg = ratings.Count > 0 ? ratings.Average(r => r.Stars) : 0;
        var userRating = userId.HasValue
            ? ratings.FirstOrDefault(r => r.UserId == userId.Value)?.Stars
            : null;

        return Ok(new
        {
            AverageRating = avg,
            TotalRatings = ratings.Count,
            UserRating = userRating
        });
    }

    [HttpDelete("{businessId}/ratings")]
    [Authorize(Roles = Constants.UserRoles.Admin)]
    public async Task<ActionResult> ResetRatings(int businessId)
    {
        var ratings = await _context.Ratings
            .Where(r => r.BusinessId == businessId)
            .ToListAsync();

        _context.Ratings.RemoveRange(ratings);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    public class RateRequest { public int Stars { get; set; } }
}