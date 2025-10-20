using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.SysRoleReportTemplateFeature.UpdateSysRoleReportTemplate
{
    public sealed record UpdateSysRoleReportTemplateRequest(int RoleId, List<ReportTemplatePermission> ReportTemplatePermissions) : IRequest;

    public sealed record ReportTemplatePermission {
        public int ReportTemplateId { get; set; }
        public int Permission { get; set; }
    }
}
