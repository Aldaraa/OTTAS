using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.DashboardFeature.EmployeeDashboard;
using tas.Application.Features.DashboardFeature.RoomDashboard;
using tas.Application.Features.DashboardFeature.TransportDashboard;
using tas.Application.Features.DashboardTransportAdminFeature.GetDomesticData;
using tas.Application.Features.DashboardTransportAdminFeature.GetInternationalTravelData;
using tas.Application.Features.DashboardTransportAdminFeature.GetRosterData;
using tas.Application.Features.DashboardTransportAdminFeature.GetTransportGroupData;
using tas.Application.Features.DashboardTransportAdminFeature.GetTransportGroupEmployeeData;

namespace tas.Application.Repositories
{
    public interface IDashboardTransportAdminRepository
    {

        Task<GetInternationalTravelDataResponse> GetInternationalTravelData(GetInternationalTravelDataRequest request, CancellationToken cancellationToken);

        Task<GetRosterDataResponse> GetRosterData(GetRosterDataRequest request, CancellationToken cancellationToken);


        Task<List<GetTransportGroupDataResponse>> GetTransportGroupData(GetTransportGroupDataRequest request, CancellationToken cancellationToken);

        Task<List<GetTransportGroupEmployeeDataResponse>> GetTransportGroupEmployeeData(GetTransportGroupEmployeeDataRequest request, CancellationToken cancellationToken);




        Task<GetDomesticDataResponse> GetDomesticData(GetDomesticDataRequest request, CancellationToken cancellationToken);



    }

}
