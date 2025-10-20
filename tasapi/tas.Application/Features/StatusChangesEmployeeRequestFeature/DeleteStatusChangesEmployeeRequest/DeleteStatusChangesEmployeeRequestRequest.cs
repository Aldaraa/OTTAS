using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using tas.Application.Features.SysRoleFeature.GetAllSysRole;

namespace tas.Application.Features.StatusChangesEmployeeRequestFeature.DeleteStatusChangesEmployeeRequest
{

    public sealed record DeleteStatusChangesEmployeeRequestRequest(int Id) : IRequest;
}
