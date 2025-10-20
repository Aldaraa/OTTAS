using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.SysRoleEmployeeReportTemplateFeature.UpdateSysRoleEmployeeReportTemplate
{
    public sealed record UpdateSysRoleEmployeeReportTemplateRequest(int EmployeeId, List<ReportTemplateEmployeePermission> ReportTemplatePermissions) : IRequest;

    public sealed record ReportTemplateEmployeePermission {
        public int ReportTemplateId { get; set; }
        public int Permission { get; set; }
    }
}
