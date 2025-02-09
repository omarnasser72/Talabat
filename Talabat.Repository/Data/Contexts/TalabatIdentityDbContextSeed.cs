using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Entities;

namespace Talabat.Repository.Data.Contexts
{
    public static class TalabatIdentityDbContextSeed
    {
        public static async Task SeedUserAsync(UserManager<AppUser> UserManager)
        {
            if (!UserManager.Users.Any())
            {
                var user = new AppUser()
                {
                    DisplayName = "Omar",
                    Email = "omar@mail.com",
                    UserName = "omar",
                    PhoneNumber = "0124489465"

                };
                await UserManager.CreateAsync(user, "Pa$$w0rd");
            }
        }
    }
}
