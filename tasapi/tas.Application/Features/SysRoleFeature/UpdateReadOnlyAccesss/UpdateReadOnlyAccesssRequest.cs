using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.SysRoleFeature.GetAllSysRole;

namespace tas.Application.Features.SysRoleFeature.UpdateReadOnlyAccesss
{

    public sealed record UpdateReadOnlyAccesssRequest(int Id,  int ReadOnlyAccess) : IRequest;
}
