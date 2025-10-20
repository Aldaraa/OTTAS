using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.DashboardSystemAdminFeature.GeOnsiteEmployeesData;
using tas.Application.Features.DashboardSystemAdminFeature.GetCampPOBData;
using tas.Application.Features.DashboardSystemAdminFeature.GetCampUtilizationData;
using tas.Application.Features.DashboardSystemAdminFeature.GetEmployeeTransportData;
using tas.Application.Features.DashboardSystemAdminFeature.GetPeopleTypeAndDepartment;
using tas.Application.Features.DashboardSystemAdminFeature.GetStatData;

namespace tas.Application.Repositories
{

    public interface IDashboardSystemAdminRepository
    {
        Task<GeOnsiteEmployeesDataResponse> GeOnsiteEmployeesData(GeOnsiteEmployeesDataRequest request, CancellationToken cancellationToken);

        Task<List<GetStatDataResponse>> GetStatData(GetStatDataRequest request, CancellationToken cancellationToken);

        Task<GetPeopleTypeAndDepartmentResponse> GetPeopleTypeAndDepartment(GetPeopleTypeAndDepartmentRequest request, CancellationToken cancellationToken);

        Task<List<GetCampPOBDataResponse>> GetCampPOBData(GetCampPOBDataRequest request, CancellationToken cancellationToken);

        Task<List<GetCampUtilizationDataResponse>> GetCampUtilizationData(GetCampUtilizationDataRequest request, CancellationToken cancellationToken);

        Task<GetEmployeeTransportDataResponse> GetEmployeeTransportData(GetEmployeeTransportDataRequest request, CancellationToken cancellationToken);






    }

}
