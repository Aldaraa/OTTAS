using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.RosterFeature.CreateRoster
{
    public sealed record CreateRosterRequest(string Name, string Description, int RosterGroupId, int Active) : IRequest;
}
