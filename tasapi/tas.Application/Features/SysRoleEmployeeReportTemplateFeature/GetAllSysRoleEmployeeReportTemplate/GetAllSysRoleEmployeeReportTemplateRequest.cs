using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.SysReportTemplateFeature.GetAllSysReportTemplate;

    namespace tas.Application.Features.SysRoleEmployeeReportTemplateFeature.GetAllSysRoleEmployeeReportTemplate
    {
        public sealed record GetAllSysRoleEmployeeReportTemplateRequest(int EmployeeId) : IRequest<List<GetAllSysRoleEmployeeReportTemplateResponse>>;

    }
