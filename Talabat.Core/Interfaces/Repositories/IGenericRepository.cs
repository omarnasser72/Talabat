using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Entities;
using Talabat.Core.Interfaces;

namespace Talabat.Core.Interfaces.Repositories
{
    public interface IGenericRepository<T> where T : BaseEntity
    {
        public Task<IReadOnlyList<T>> GetAllAsync();
        public Task<T?> GetById(int id);

        public Task<IReadOnlyList<T>> GetAllAsyncWithSpec(ISpecification<T> Specification);
        public Task<T?> GetByIdWithSpec(ISpecification<T> Specification);
    }
}
