using MediatR;

namespace tas.Application.Features.RequestDocumentFeature.GetNonSiteTravelGroup
{
    public sealed record GetNonSiteTravelGroupRequest : IRequest<List<GetNonSiteTravelGroupResponse>>;
}
