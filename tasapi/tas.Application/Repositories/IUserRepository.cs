using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Domain.Entities;

namespace tas.Application.Repositories
{
    public interface IUserRepository : IBaseRepository<User>
    {
        Task<User> GetByEmail(string email, CancellationToken cancellationToken);

        Task<User> GetNameFilter(string name, CancellationToken cancellationToken);
    }
}
