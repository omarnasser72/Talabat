using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Talabat.Core.Entities;

namespace Talabat.APIs.Extensions
{
    public static class UserManagerExtensions
    {
        public static Task<AppUser?> FindUserWithAddressAsync(this UserManager<AppUser> userManager, ClaimsPrincipal User)
        {
            var email = User.FindFirstValue(ClaimTypes.Email);
            var user = userManager.Users.Include(user => user.Address)
                                        .FirstOrDefaultAsync(user => user.Email == email);
            return user;
        }
    }
}
