using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.RosterGroupFeature.UpdateRosterGroup
{
    public sealed record UpdateRosterGroupRequest(int Id,  string Description, int Active) : IRequest;
}
