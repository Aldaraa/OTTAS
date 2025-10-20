using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.RosterGroupFeature.DeleteRosterGroup
{
    public sealed record DeleteRosterGroupRequest(int Id) : IRequest;
}
