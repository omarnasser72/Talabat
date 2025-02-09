using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Talabat.Core.Entities;

namespace Talabat.Repository.Data.Contexts
{
    public static class TalabatDbContextSeed
    {
        public static async Task SeedAsync(TalabatDbContext DbContext)
        {
            #region ProductBrand Seeding
            if (!DbContext.Set<ProductBrand>().Any())
            {
                var BrandsData = File.ReadAllText("../Talabat.Repository/Data/DataSeed/brands.json");
                var Brands = JsonSerializer.Deserialize<List<ProductBrand>>(BrandsData);

                if (Brands?.Count > 0)
                    foreach (var Brand in Brands)
                        await DbContext.Set<ProductBrand>().AddAsync(Brand);

                await DbContext.SaveChangesAsync();
            }
            #endregion

            #region ProductType Seeding
            if (!DbContext.Set<ProductType>().Any())
            {
                var TypesData = File.ReadAllText("../Talabat.Repository/Data/DataSeed/types.json");
                var Types = JsonSerializer.Deserialize<List<ProductType>>(TypesData);

                if (Types?.Count > 0)
                    foreach (var Type in Types)
                        await DbContext.Set<ProductType>().AddAsync(Type);

                await DbContext.SaveChangesAsync();
            }
            #endregion

            #region Product Seeding
            if (!DbContext.Set<Product>().Any())
            {
                var ProductsData = File.ReadAllText("../Talabat.Repository/Data/DataSeed/products.json");
                var Products = JsonSerializer.Deserialize<List<Product>>(ProductsData);

                if (Products?.Count > 0)
                    foreach (var Product in Products)
                        await DbContext.Set<Product>().AddAsync(Product);

                await DbContext.SaveChangesAsync();
            }
            #endregion
        }
    }
}
