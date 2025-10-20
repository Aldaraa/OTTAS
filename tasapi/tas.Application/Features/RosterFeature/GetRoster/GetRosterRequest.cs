using MediatR;
using tas.Application.Features.RosterFeature.GetRoster;

namespace tas.Application.Features.RosterFeature.GetRoster
{
    public sealed record GetRosterRequest(int Id) : IRequest<GetRosterResponse>;
}
