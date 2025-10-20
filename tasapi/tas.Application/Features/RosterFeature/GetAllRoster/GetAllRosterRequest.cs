using MediatR;

namespace tas.Application.Features.RosterFeature.GetAllRoster
{
    public sealed record GetAllRosterRequest(int? status) : IRequest<List<GetAllRosterResponse>>;
}
