using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Entities;

namespace Talabat.Repository.Data.Contexts
{
    public class TalabatIdentityDbContext : IdentityDbContext<AppUser>
    {
        public TalabatIdentityDbContext(DbContextOptions<TalabatIdentityDbContext> options) : base(options) { }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<AppUser>()
                .HasOne(a => a.Address)
                .WithOne(ad => ad.AppUser)
                .HasForeignKey<Address>(ad => ad.AppUserId);
        }

    }
}
