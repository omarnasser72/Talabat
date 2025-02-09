using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Entities;

namespace Talabat.Repository.Data.Configurations
{
    public class ProductConfigurations : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.HasOne(P => P.ProductBrand)
                   .WithMany()
                   .HasForeignKey(P => P.ProductBrandId);

            builder.HasOne(P => P.ProductType)
                   .WithMany()
                   .HasForeignKey(P => P.ProductTypeId);

            #region Properties Configurations
            builder.Property(P => P.Name)
                   .IsRequired();

            builder.Property(P => P.Description)
                   .IsRequired();

            builder.Property(P => P.PictureUrl)
                   .IsRequired();
            #endregion
        }
    }
}
