using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.DashboardDataAdminFeature.GetEmployeeRegisterData;
using tas.Application.Features.DashboardDataAdminFeature.GetOnsiteEmployeesData;
using tas.Application.Features.DashboardDataAdminFeature.GetPackMealData;
using tas.Application.Features.DashboardDataAdminFeature.GetProfileChangeDepartmentData;
using tas.Application.Features.DashboardDataAdminFeature.GetSeatBlockOnsiteData;
using tas.Application.Features.DashboardDataAdminFeature.GetTransportData;

namespace tas.Application.Repositories
{
   public interface IDashboardDataAdminRepository
   {
        Task<List<GetPackMealDataResponse>> GetPackMealData(GetPackMealDataRequest request, CancellationToken cancellationToken);

        Task<List<GetEmployeeRegisterDataResponse>> GetEmployeeRegisterData(GetEmployeeRegisterDataRequest request, CancellationToken cancellationToken);
        Task<GetOnsiteEmployeesDataResponse> GetOnsiteEmployeesData(GetOnsiteEmployeesDataRequest request, CancellationToken cancellationToken);


        Task<GetProfileChangeDepartmentDataResponse> GetProfileChangeDepartmentData(GetProfileChangeDepartmentDataRequest request, CancellationToken cancellationToken);

        Task<List<GetSeatBlockOnsiteDataResponse>> GetSeatBlockOnsiteData(GetSeatBlockOnsiteDataRequest request, CancellationToken cancellationToken);


        Task<GetTransportDataResponse> GetTransportData(GetTransportDataRequest request, CancellationToken cancellationToken);



   }
}
