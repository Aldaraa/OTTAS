using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.SysRoleEmployeeReportTemplateFeature.GetAllSysRoleEmployeeReportTemplate;
using tas.Application.Features.SysRoleEmployeeReportTemplateFeature.UpdateSysRoleEmployeeReportTemplate;
using tas.Application.Repositories;
using tas.Domain.Entities;

namespace tas.Application.Repositories
{
  public  interface  ISysRoleEmployeeReportTemplateRepository : IBaseRepository<SysRoleEmployeeReportTemplate>
    {
        Task UpdateReportTemplateRole(UpdateSysRoleEmployeeReportTemplateRequest request, CancellationToken cancellationToken);

        Task<List<GetAllSysRoleEmployeeReportTemplateResponse>> GetEmployeeReportTemplate(GetAllSysRoleEmployeeReportTemplateRequest request, CancellationToken cancellationToken);
    }
}

