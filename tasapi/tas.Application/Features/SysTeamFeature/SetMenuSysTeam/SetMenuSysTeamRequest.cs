using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.SysTeamFeature.SetMenuSysTeam
{
    public sealed record SetMenuSysTeamBulkRequest(List<SetMenuSysTeamRequest> Requests) : IRequest;
    public sealed record SetMenuSysTeamRequest(int TeamId, int MenuId, bool Permission) : IRequest;
}
