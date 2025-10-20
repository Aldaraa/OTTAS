using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.SysTeamFeature.DeleteUserSysTeam
{
    public sealed record DeleteUserSysTeamRequest(int TeamId, int UserId) : IRequest;
}
