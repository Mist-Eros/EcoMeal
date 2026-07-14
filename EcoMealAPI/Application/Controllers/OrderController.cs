using System.Security.Claims;
using EcoMeal.EcoMealAPI.Constants;
using EcoMeal.EcoMealAPI.Entities;
using EcoMeal.EcoMealAPI.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
    public async Task<ActionResult<OrderGetDTO>> PlaceOrder([FromBody] OrderCreateDTO request)
    {
        var userId = GetCurrentUserId();

        var package = await _context.Package
            .Include(p => p.Business)
            .Include(p => p.Orders)
            .FirstOrDefaultAsync(p => p.Id == request.PackageId);

        if (package is null)
            return NotFound("Package was not found.");

        if (package.Orders.Any())
            return BadRequest("Package is not available.");

        var order = new Order
        {
            UserId = userId,
            PackageId = package.Id,
            Status = 1,
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
        });
    }

    [HttpGet("my")]
    public async Task<ActionResult<IEnumerable<OrderGetDTO>>> GetMyOrders()
    {
        var userId = GetCurrentUserId();
        var orders = await _context.Orders
            .Where(o => o.UserId == userId && o.Status == 1)
            .OrderByDescending(o => o.Date)
            .Select(o => new OrderGetDTO
            {
                Id = o.Id,
                Date = o.Date,
                Status = o.Status,
                Price = o.Package.Price,
                BusinessId = o.Package.BusinessId,
                BusinessName = o.Package.Business.Name,
                PackageName = o.Package.Name,
                UserName = o.User.Name,
                UserContact = o.User.Contact,
                UserEmail = o.User.Email,
            })
            .ToListAsync();

        return orders;
    }

    [HttpGet("all")]
    [Authorize(Roles = UserRoles.Admin)]
    public async Task<ActionResult<IEnumerable<OrderGetDTO>>> GetAllOrders()
    {
        var orders = await _context.Orders
            .Where(o => o.Status == 1)
            .OrderByDescending(o => o.Date)
            .Select(o => new OrderGetDTO
            {
                Id = o.Id,
                Date = o.Date,
                Status = o.Status,
                Price = o.Package.Price,
                BusinessId = o.Package.BusinessId,
                BusinessName = o.Package.Business.Name,
                PackageName = o.Package.Name,
                UserName = o.User.Name,
                UserContact = o.User.Contact,
                UserEmail = o.User.Email,
            })
            .ToListAsync();

        return orders;
    }

    [HttpGet("history")]
    public async Task<ActionResult<IEnumerable<OrderGetDTO>>> GetOrderHistory()
    {
        var userId = GetCurrentUserId();
        var isAdmin = User.IsInRole(UserRoles.Admin);

        var query = _context.Orders
            .Where(o => o.Status != 0);

        if (!isAdmin)
            query = query.Where(o => o.UserId == userId);

        var orders = await query
            .OrderByDescending(o => o.Date)
            .Select(o => new OrderGetDTO
            {
                Id = o.Id,
                Date = o.Date,
                Status = o.Status,
                Price = o.Package.Price,
                BusinessId = o.Package.BusinessId,
                BusinessName = o.Package.Business.Name,
                PackageName = o.Package.Name,
                UserName = o.User.Name,
                UserContact = o.User.Contact,
                UserEmail = o.User.Email,
            })
            .ToListAsync();

        return orders;
    }

    [HttpPut("{id}/pickup")]
    public async Task<ActionResult> MarkPickedUp(int id)
    {
        var userId = GetCurrentUserId();
        var isAdmin = User.IsInRole(UserRoles.Admin);

        var order = await _context.Orders.FindAsync(id);
        if (order is null)
            return NotFound("Order not found.");

        if (!isAdmin && order.UserId != userId)
            return Forbid();

        order.Status = 2;
        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpPut("{id}/cancel")]
    public async Task<ActionResult> CancelOrder(int id)
    {
        var userId = GetCurrentUserId();
        var isAdmin = User.IsInRole(UserRoles.Admin);

        var order = await _context.Orders.FindAsync(id);
        if (order is null)
            return NotFound("Order not found.");

        if (!isAdmin && order.UserId != userId)
            return Forbid();

        order.Status = 0;
        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpGet("discount")]
    public async Task<ActionResult<object>> GetDiscount()
    {
        var userId = GetCurrentUserId();
        var pendingCount = await _context.Orders
            .CountAsync(o => o.UserId == userId && o.Status == 1);

        var discountPercent = pendingCount * 5;

        return Ok(new
        {
            OrderCount = pendingCount,
            DiscountPercent = discountPercent
        });
    }

    [HttpPut("pickup/all")]
    public async Task<ActionResult> MarkAllPickedUp()
    {
        var userId = GetCurrentUserId();
        var orders = await _context.Orders
            .Where(o => o.UserId == userId && o.Status == 1)
            .ToListAsync();

        foreach (var o in orders)
            o.Status = 2;

        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete]
    public async Task<ActionResult> ClearMyOrders()
    {
        var userId = GetCurrentUserId();
        await _context.Orders
            .Where(o => o.UserId == userId)
            .ExecuteDeleteAsync();

        return NoContent();
    }

    private int GetCurrentUserId()
    {
        var userIdValue = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return int.Parse(userIdValue!);
    }
}
