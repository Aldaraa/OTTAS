using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.CampFeature.GetAllCamp;
using tas.Application.Features.DashboardFeature.EmployeeDashboard;
using tas.Application.Features.DashboardFeature.RoomDashboard;
using tas.Application.Features.DashboardFeature.TransportDashboard;
using tas.Domain.Entities;

namespace tas.Application.Repositories
{
    public interface IDashboardRepository 
    {
        Task<EmployeeDashboardResponse> EmployeeData(EmployeeDashboardRequest request, CancellationToken cancellation);
        Task<RoomDashboardResponse> RoomData(RoomDashboardRequest request, CancellationToken cancellation);

        Task<TransportDashboardResponse> TransportData(TransportDashboardRequest request, CancellationToken cancellation);




    }


}