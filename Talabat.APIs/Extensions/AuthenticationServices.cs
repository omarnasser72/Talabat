using Microsoft.AspNetCore.Identity;
using Talabat.Core.Entities;
using Talabat.Repository.Data.Contexts;

namespace Talabat.APIs.Extensions
{
    public static class AuthenticationServices
    {
        public static IServiceCollection AddAuthenticationServices(this IServiceCollection services)
        {
            services.AddAuthentication(); //UserManager, SigninManager, RoleManger
            services.AddIdentity<AppUser, IdentityRole>()
                            .AddEntityFrameworkStores<TalabatIdentityDbContext>();
            return services;
        }
    }
}
