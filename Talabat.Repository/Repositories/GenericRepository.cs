using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Entities;
using Talabat.Core.Interfaces;
using Talabat.Core.Interfaces.Repositories;
using Talabat.Repository.Data.Contexts;
using Talabat.Repository.Specifications;

namespace Talabat.Repository.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : BaseEntity
    {
        private readonly TalabatDbContext DbContext;

        public GenericRepository(TalabatDbContext DbContext)
        {
            this.DbContext = DbContext;
        }

        public async Task<IReadOnlyList<T>> GetAllAsync()
            => await DbContext.Set<T>().ToListAsync();

        public async Task<T?> GetById(int id)
            => await DbContext.Set<T>().FindAsync(id);



        private IQueryable<T> ApplySpecification(ISpecification<T> Specification)
        {
            return SpecificationEvaluator<T>.GetQuery(DbContext.Set<T>(), Specification);
        }

        public async Task<IReadOnlyList<T>> GetAllAsyncWithSpec(ISpecification<T> Specification)
        {
            return await ApplySpecification(Specification).ToListAsync();
        }

        public async Task<T?> GetByIdWithSpec(ISpecification<T> Specification)
        {
            return await ApplySpecification(Specification).FirstOrDefaultAsync();
        }

    }
}
