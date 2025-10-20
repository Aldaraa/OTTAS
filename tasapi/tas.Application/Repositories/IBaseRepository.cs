using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using tas.Domain.Common;

namespace tas.Application.Repositories
{
    public interface IBaseRepository<T> where T : BaseEntity
    {
        void Create(T entity);
        void Update(T entity);
        void Delete(T entity);

        Task Patch(T entity);
        Task<T> Get(int id, CancellationToken cancellationToken);
        Task<List<T>> GetAll(CancellationToken cancellationToken);
        Task<List<T>> GetAllActiveFilter(int status, CancellationToken cancellationToken);

        Task CheckDuplicateData(T entity, params Expression<Func<T, Object>>[] properties);

    }
}
