using EcoMeal.EcoMealAPI.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace EcoMeal.EcoMealAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class UsersController : ControllerBase
    {
        private readonly UserManager<User> _userManager;

        public UsersController(UserManager<User> userManager)
        {
            _userManager = userManager;
        }

        [HttpPut("{email}/role")]
        public async Task<IActionResult> UpdateUserRole(string email, [FromBody] UpdateRoleRequest request)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
                return NotFound(new { Message = $"User with email '{email}' not found" });

            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
                return Unauthorized(new { Message = "You are not logged in" });

            if (currentUser.Email != email)
                return BadRequest(new { Message = "You can only change your own role" });

            var currentRoles = await _userManager.GetRolesAsync(user);
            if (currentRoles.Any())
            {
                var removeResult = await _userManager.RemoveFromRolesAsync(user, currentRoles);
                if (!removeResult.Succeeded)
                    return BadRequest(new { Message = string.Join(", ", removeResult.Errors.Select(e => e.Description)) });
            }

            var addResult = await _userManager.AddToRoleAsync(user, request.Role);
            if (addResult.Succeeded)
                return Ok(new { Message = $"Role updated to '{request.Role}' successfully" });

            return BadRequest(new { Message = string.Join(", ", addResult.Errors.Select(e => e.Description)) });
        }
    }

    public class UpdateRoleRequest
    {
        public string Role { get; set; }
    }
}