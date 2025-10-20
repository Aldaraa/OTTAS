using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.DepartmentCostCodeFeature.AddDepartmentCostCode;
using tas.Application.Features.DepartmentCostCodeFeature.DeleteDepartmentCostCode;
using tas.Application.Features.EmployerAdminFeature.AddEmployerAdmin;
using tas.Application.Features.EmployerAdminFeature.DeleteEmployerAdmin;
using tas.Application.Features.EmployerAdminFeature.GetEmployerAdmin;
using tas.Application.Features.SysRoleEmployeeReportEmployerFeature.AddSysRoleEmployeeReportEmployer;
using tas.Application.Features.SysRoleEmployeeReportEmployerFeature.DeleteSysRoleEmployeeReportEmployer;
using tas.Application.Features.SysRoleEmployeeReportEmployerFeature.GetSysRoleEmployeeReportEmployer;
using tas.Domain.Entities;

namespace tas.Application.Repositories
{
    public interface IEmployerAdminRepository : IBaseRepository<EmployerAdmin>
    {

        Task AddEmployerAdmin(AddEmployerAdminRequest request, CancellationToken cancellationToken);

        Task DeleteEmployerAdmin(DeleteEmployerAdminRequest request, CancellationToken cancellationToken);

        Task<List<GetEmployerAdminResponse>> GetEmployerAdmin(GetEmployerAdminRequest request, CancellationToken cancellationToken);





    }

}
