
using tas.Domain.Entities;

namespace tas.Application.Repositories
{
    public interface ILocationRepository : IBaseRepository<Location>
    {
        Task<Location> GetbyId(int Id, CancellationToken cancellationToken);
    }
}
