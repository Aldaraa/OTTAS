using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.SysRoleFeature.GetAllSysRole;

namespace tas.Application.Features.SysRoleFeature.AddEmployee
{

    public sealed record AddEmployeeRequest(int EmployeeId, int RoleId, int ReadOnlyAccess) : IRequest;
}
