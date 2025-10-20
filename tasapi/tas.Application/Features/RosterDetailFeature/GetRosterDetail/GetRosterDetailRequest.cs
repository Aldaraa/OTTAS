using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.RosterDetailFeature.GetRosterDetail
{
    public sealed record GetRosterDetailRequest(int RosterId) : IRequest<List<GetRosterDetailResponse>>;
}
