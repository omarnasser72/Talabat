using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Entities;
using Talabat.Core.Interfaces;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Talabat.Repository.Specifications
{
    public static class SpecificationEvaluator<T> where T : BaseEntity
    {
        //function to build query
        //DbContext.Products.Where(P => P.Id == id).Include(P => P.ProductType).Include(P => P.ProductBrand);
        public static IQueryable<T> GetQuery(IQueryable<T> InputQuery, ISpecification<T> Specification)
        {
            var Query = InputQuery; //DbContext.Products
            if (Specification.Criteria != null)
                Query = Query.Where(Specification.Criteria);    //Where(P => P.Id == id)

            if (Specification.OrderByAsc != null)
                Query = Query.OrderBy(Specification.OrderByAsc);

            if (Specification.OrderByDesc != null)
                Query = Query.OrderByDescending(Specification.OrderByDesc);

            if (Specification.IsPaginationEnabled)
                Query = Query.Skip(Specification.Skip).Take(Specification.Take);


            //Include(P => P.ProductType).Include(P => P.ProductBrand)

            //CurrentCriteria: This is the result of the aggregation up to the current point (starts as Query and gets updated with each iteration).
            #region Simple Example
            /*
             * int[] numbers = { 1, 2, 3, 4, 5 };
             * int sum = numbers.Aggregate(0, (acc, number) => acc + number);
             * sum = 15
             */
            #endregion

            Query = Specification
                    .Includes
                    .Aggregate(Query, (AccumulatedCriteria, IncludeExpression) => AccumulatedCriteria.Include(IncludeExpression));
            //DbContext.Products.Where(P => P.Id == id).Include(P => P.ProductType);
            //DbContext.Products.Where(P => P.Id == id).Include(P => P.ProductType).Include(P => P.ProductBrand);

            return Query;
        }
    }
}
