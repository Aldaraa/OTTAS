using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.SysRoleEmployeeReportEmployerFeature.AddSysRoleEmployeeReportEmployer;
using tas.Application.Features.SysRoleEmployeeReportEmployerFeature.DeleteSysRoleEmployeeReportEmployer;
using tas.Application.Features.SysRoleEmployeeReportEmployerFeature.GetSysRoleEmployeeReportEmployer;
using tas.Domain.Entities;

namespace tas.Application.Repositories
{
    public interface ISysRoleEmployeeReportEmployerRepository : IBaseRepository<SysRoleEmployeeReportEmployer>
    {
        Task AddSysRoleEmployeeReportEmployer(AddSysRoleEmployeeReportEmployerRequest request, CancellationToken cancellationToken);

        Task DeleteSysRoleEmployeeReportEmployer(DeleteSysRoleEmployeeReportEmployerRequest request, CancellationToken cancellationToken);

        Task<List<GetSysRoleEmployeeReportEmployerResponse>> GetData(GetSysRoleEmployeeReportEmployerRequest request, CancellationToken cancellationToken);



    }

}
