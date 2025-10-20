using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.RosterDetailFeature.DeleteRosterDetail
{
    public sealed record DeleteRosterDetailRequest(int Id) : IRequest;
}
