using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.RosterDetailFeature.UpdateSeqRosterDetail
{
    public sealed record UpdateSeqRosterDetailRequest(List<int> RosterDetailIds, int RosterId) : IRequest;
}
