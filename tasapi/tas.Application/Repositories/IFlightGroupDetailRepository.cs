using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.FlightGroupDetailFeature.SetClusterFlightGroupDetail;
using tas.Application.Features.FlightGroupDetailFeature.UpdateShiftFlightGroupDetail;
using tas.Domain.Entities;

namespace tas.Application.Repositories
{
    public interface IFlightGroupDetailRepository : IBaseRepository<FlightGroupDetail>
    {
       // Task<FlightGroupDetail> GetbyId(int Id, CancellationToken cancellationToken);

        Task SetCluster(SetClusterFlightGroupDetailRequest request, CancellationToken cancellationToken);

        Task UpdateShift(UpdateShiftFlightGroupDetailRequest request, CancellationToken cancellationToken);


    }
}
