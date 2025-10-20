using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.SysReportTemplateFeature.GetAllSysReportTemplate;

    namespace tas.Application.Features.SysRoleReportTemplateFeature.GetAllSysRoleReportTemplate
    {
        public sealed record GetAllSysRoleReportTemplateRequest(int RoleId) : IRequest<List<GetAllSysRoleReportTemplateResponse>>;

    }
