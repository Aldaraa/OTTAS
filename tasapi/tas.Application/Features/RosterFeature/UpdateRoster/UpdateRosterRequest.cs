 using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.RosterFeature.UpdateRoster
{
    public sealed record UpdateRosterRequest(int Id, string Name,string Description, int RosterGroupId, int Active) : IRequest;
}
