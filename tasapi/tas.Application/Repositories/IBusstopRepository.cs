using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.BedFeature.GetAllBed;
using tas.Application.Features.BedFeature.GetBed;
using tas.Application.Features.BusstopFeature.DeleteBusstop;
using tas.Application.Features.BusstopFeature.GetAllBusstop;
using tas.Domain.Entities;

namespace tas.Application.Repositories
{


    public interface IBusstopRepository : IBaseRepository<Busstop>
    {
        Task<List<GetAllBusstopResponse>> GetAllBusstop(GetAllBusstopRequest request, CancellationToken cancellationToken);

        Task DeletForce(DeleteBusstopRequest request, CancellationToken cancellationToken);


    }
}
