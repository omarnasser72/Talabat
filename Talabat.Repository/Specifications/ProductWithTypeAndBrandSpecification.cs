using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Entities;
using Talabat.Core.Specifications;

namespace Talabat.Repository.Specifications
{
    public class ProductWithTypeAndBrandSpecification : BaseSpecification<Product>
    {
        public ProductWithTypeAndBrandSpecification(ProductParamsSpecification Params)
        /*
         * This condition means:
         * (!Params.BrandId.HasValue || P.ProductBrandId == Params.BrandId):
         *      If BrandId is not provided (!Params.BrandId.HasValue), the condition returns true 
         *      (no filtering on ProductBrandId).
         *      If BrandId is provided, it checks if ProductBrandId equals BrandId.
         *
         * (!Params.ProductTypeId.HasValue || P.ProductTypeId == Params.ProductTypeId):
         *      If ProductTypeId is not provided, the condition returns true 
         *      (no filtering on ProductTypeId).
         *      If ProductTypeId is provided, it checks if ProductTypeId equals ProductTypeId.
         */
        : base(P => (!Params.BrandId.HasValue || P.ProductBrandId == Params.BrandId)
                     &&
                    (!Params.ProductTypeId.HasValue || P.ProductTypeId == Params.ProductTypeId))
        {
            Includes.Add(P => P.ProductType);
            Includes.Add(P => P.ProductBrand);
            if (!string.IsNullOrEmpty(Params.Sort))
            {
                switch (Params.Sort)
                {
                    case "PriceAsc":
                        AddOrderByAsc(P => P.Price);
                        break;
                    case "PriceDesc":
                        AddOrderByDesc(P => P.Price);
                        break;
                    default:
                        AddOrderByAsc(P => P.Name);
                        break;
                }
            }

            AddPagination(Params.PageSize * Params.PageIndex, Params.PageSize);

        }
        public ProductWithTypeAndBrandSpecification(int Id) : base(P => P.Id == Id)
        {
            Includes.Add(P => P.ProductType);
            Includes.Add(P => P.ProductBrand);
        }

    }
}
