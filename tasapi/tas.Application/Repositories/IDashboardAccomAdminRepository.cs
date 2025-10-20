using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.DashboardAccomAdminFeature.GetCampInfo;
using tas.Application.Features.DashboardAccomAdminFeature.GetCampUsageInfo;
using tas.Application.Features.DashboardAccomAdminFeature.GetNonSiteInfo;
using tas.Application.Features.DashboardAccomAdminFeature.GetNoRoomInfo;
using tas.Application.Features.DashboardAccomAdminFeature.GetOccupantsInfo;
using tas.Application.Features.DashboardAccomAdminFeature.GetPobInfo;
using tas.Application.Features.DashboardDataAdminFeature.GetEmployeeRegisterData;
using tas.Application.Features.DashboardDataAdminFeature.GetOnsiteEmployeesData;
using tas.Application.Features.DashboardDataAdminFeature.GetPackMealData;
using tas.Application.Features.DashboardDataAdminFeature.GetProfileChangeDepartmentData;
using tas.Application.Features.DashboardDataAdminFeature.GetSeatBlockOnsiteData;
using tas.Application.Features.DashboardDataAdminFeature.GetTransportData;

namespace tas.Application.Repositories
{
    public interface IDashboardAccomAdminRepository
    {

        Task<GetCampInfoResponse> GetCampInfo(GetCampInfoRequest request, CancellationToken cancellationToken);

        Task<GetCampUsageInfoResponse> GetCampUsageInfo(GetCampUsageInfoRequest request, CancellationToken cancellationToken);

        Task<GetNonSiteInfoResponse> GetNonSiteInfo(GetNonSiteInfoRequest request, CancellationToken cancellationToken);


        Task<GetNoRoomInfoResponse> GetNoRoomInfo(GetNoRoomInfoRequest request, CancellationToken cancellationToken);

        Task<GetPobInfoResponse> GetPobInfo(GetPobInfoRequest request, CancellationToken cancellationToken);

        Task<GetOccupantsInfoResponse> GetOccupantsInfo(GetOccupantsInfoRequest request, CancellationToken cancellationToken);





    }
}
