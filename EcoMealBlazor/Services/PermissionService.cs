using System.Security.Claims;

namespace EcoMeal.EcoMealBlazor.Services
{
    public class PermissionService
    {
        public bool CanEdit(ClaimsPrincipal user) => user.IsInRole("Admin");
        public bool CanDelete(ClaimsPrincipal user) => user.IsInRole("Admin");
        public bool CanAdd(ClaimsPrincipal user) => user.IsInRole("Admin");
        public bool CanOrder(ClaimsPrincipal user) => user.IsInRole("User") || user.IsInRole("Admin");
        public bool IsAdmin(ClaimsPrincipal user) => user.IsInRole("Admin");
        public bool IsUser(ClaimsPrincipal user) => user.IsInRole("User");
    }
}