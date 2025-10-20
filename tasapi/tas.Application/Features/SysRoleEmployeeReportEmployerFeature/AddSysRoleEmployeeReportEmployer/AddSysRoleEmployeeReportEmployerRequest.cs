using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.UserFeatures.GetAllUser;

namespace tas.Application.Features.SysRoleEmployeeReportEmployerFeature.AddSysRoleEmployeeReportEmployer
{
    public sealed record AddSysRoleEmployeeReportEmployerRequest(int EmployeeId, int EmployerId) : IRequest<Unit>;
}
