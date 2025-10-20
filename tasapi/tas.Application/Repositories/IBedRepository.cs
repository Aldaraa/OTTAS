using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.BedFeature.GetAllBed;
using tas.Application.Features.BedFeature.GetBed;
using tas.Domain.Entities;

namespace tas.Application.Repositories
{

    public interface IBedRepository : IBaseRepository<Bed>
    {
        
        Task<List<GetAllBedResponse>> GetAllBedFilter(int? status, int roomId, CancellationToken cancellationToken);
        Task<List<GetAllBedResponse>>GetAllBed(int roomId, CancellationToken cancellationToken);
        Task<GetBedResponse> Get(int Id, CancellationToken cancellationToken);

    }
}
