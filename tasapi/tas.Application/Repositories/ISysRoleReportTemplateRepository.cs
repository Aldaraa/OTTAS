using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.SysReportTemplateFeature.GetAllSysReportTemplate;
using tas.Application.Features.SysRoleMenuFeature.GetAllSysRoleMenu;
using tas.Application.Features.SysRoleMenuFeature.UpdateSysRoleMenu;
using tas.Application.Features.SysRoleReportTemplateFeature.GetAllSysRoleReportTemplate;
using tas.Application.Features.SysRoleReportTemplateFeature.UpdateSysRoleReportTemplate;
using tas.Domain.Entities;

namespace tas.Application.Repositories
{
    public interface  ISysRoleReportTemplateRepository  : IBaseRepository<SysRoleReportTemplate>
    {
        Task UpdateReportTemplateRole(UpdateSysRoleReportTemplateRequest request, CancellationToken cancellationToken);

        Task<List<GetAllSysRoleReportTemplateResponse>> GetRoleReportTemplate(GetAllSysRoleReportTemplateRequest request, CancellationToken cancellationToken);
    }
}
