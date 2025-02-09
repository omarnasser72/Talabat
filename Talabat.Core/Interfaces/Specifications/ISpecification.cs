using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Entities;

namespace Talabat.Core.Interfaces
{
    public interface ISpecification<T> where T : BaseEntity
    {
        //DbContext.Products.Where(P => P.Id == id).Include(P => P.ProductType).Include(P => P.ProductBrand);

        //Signature for property where expression [ Where(P => P.Id == id) ]
        public Expression<Func<T, bool>>? Criteria { get; set; }      //Expression represents lambda expression

        //Signature for property for list of includes Include(P => P.ProductType).Include(P => P.ProductBrand) => List Of Includes
        public List<Expression<Func<T, object>>> Includes { get; set; }


        public Expression<Func<T, object>>? OrderByAsc { get; set; }
        public Expression<Func<T, object>>? OrderByDesc { get; set; }
        public int Take { get; set; }
        public int Skip { get; set; }
        public bool IsPaginationEnabled { get; set; }
    }
}
