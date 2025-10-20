using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.RosterFeature.DeleteRoster
{
    public sealed record DeleteRosterRequest(int Id) : IRequest;
}
