using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.FlightGroupMasterFeature.GetAllFlightGroupMaster;
using tas.Application.Features.FlightGroupMasterFeature.GetFlightGroupMaster;
using tas.Domain.Entities;

namespace tas.Application.Repositories
{

    public interface IFlightGroupMasterRepository : IBaseRepository<FlightGroupMaster>
    {
        Task<FlightGroupMaster> GetbyId(int Id, CancellationToken cancellationToken);

        Task<GetFlightGroupMasterResponse> GetProfile(GetFlightGroupMasterRequest request, CancellationToken cancellationToken);

        Task CreateShiftConfig(int FlightGroupMasterId, int DayPattern);

        Task<List<GetAllFlightGroupMasterResponse>> GetAllData(GetAllFlightGroupMasterRequest request, CancellationToken cancellationToken);
    }
}
