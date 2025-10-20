using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.SysRoleEmployeeReportDepartmentFeature.AddSysRoleEmployeeReportDepartment;
using tas.Application.Features.SysRoleEmployeeReportDepartmentFeature.DeleteSysRoleEmployeeReportDepartment;
using tas.Application.Features.SysRoleEmployeeReportDepartmentFeature.GetSysRoleEmployeeReportDepartment;
using tas.Domain.Entities;

namespace tas.Application.Repositories
{

    public interface ISysRoleEmployeeReportDepartmentRepository : IBaseRepository<SysRoleEmployeeReportDepartment>
    {
        Task AddSysRoleEmployeeReportDepartment(AddSysRoleEmployeeReportDepartmentRequest request, CancellationToken cancellationToken);

        Task DeleteSysRoleEmployeeReportDepartment(DeleteSysRoleEmployeeReportDepartmentRequest request, CancellationToken cancellationToken);

        Task<List<GetSysRoleEmployeeReportDepartmentResponse>> GetData(GetSysRoleEmployeeReportDepartmentRequest request, CancellationToken cancellationToken);



    }

}
