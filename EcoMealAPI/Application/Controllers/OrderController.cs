using System.Reflection.Metadata.Ecma335;
using System.Security.Claims;
using EcoMeal.EcoMealAPI.Entities;
using EcoMeal.EcoMealAPI.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using EcoMeal.EcoMealAPI.Models;

namespace EcoMeal.EcoMealAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class OrderController : ControllerBase
{
    private readonly EcoMealDBContext _context;
    public OrderController(EcoMealDBContext context)
    {
        _context = context;
    }

    [HttpPost]
    public async Task<ActionResult<OrderGetDTO>> PlaceOrder ([FromBodyAttribute] OrderCreateDTO request)
    {
        var userId = GetCurrentUserId();

        var package = await _context.Package
        .Include(p => p.Business)
        .Include(p => p.Orders)
        .FirstOrDefaultAsync(p => p.Id == request.PackageId);

        if (package is null)
        {
            return NotFound("Package was not found.");
        }

        if (package.Orders.Any())
        {
            return BadRequest("Package is not available.");
        }

        var order = new Order
        {
            UserId = userId,
            PackageId = package.Id,
            Status = 1,
            /* 
            status - 1 = placed
            */
            Date = DateTime.UtcNow,
        };

        _context.Orders.Add(order);
        await _context.SaveChangesAsync();

        return Ok(new OrderGetDTO
        {
            Id = order.Id,
            Date = order.Date,
            Status = order.Status,
            PackageName = package.Name,
            Price = package.Price,
            BusinessId = package.BusinessId,
            BusinessName = package.Business.Name,
            UserName = order.User?.Name,
            UserContact = order.User?.Contact
        });
    }

    [HttpGet("my")]
    public async Task<ActionResult<IEnumerable<OrderGetDTO>>> GetMyOrders()
    {
        var userId = GetCurrentUserId();
        var orders = await _context.Orders
        .Where ( o => o.UserId == userId)
        .OrderByDescending (o => o.Date)
        .Select (o => new OrderGetDTO
        {
            Id = o.Id,
            Date = o.Date,
            Status = o.Status,
            Price = o.Package.Price,
            BusinessId = o.Package.BusinessId,
            BusinessName = o.Package.Business.Name,
            PackageName = o.Package.Name
        })
        .ToListAsync();

        return orders;
    }

    [HttpDelete]
    public async Task<ActionResult> ClearMyOrders()
    {
        var userId = GetCurrentUserId();
        var orders = await _context.Orders
            .Where(o => o.UserId == userId)
            .ToListAsync();

        _context.Orders.RemoveRange(orders);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private int GetCurrentUserId()
    {
        var userIdValue = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return int.Parse(userIdValue!);
    }
}