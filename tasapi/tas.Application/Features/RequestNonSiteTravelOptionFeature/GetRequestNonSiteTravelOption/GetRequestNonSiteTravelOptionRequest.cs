using MediatR;

namespace tas.Application.Features.RequestNonSiteTravelOptionFeature.GetRequestNonSiteTravelOption
{
    public sealed record GetRequestNonSiteTravelOptionRequest(int DocumentId) : IRequest<List<GetRequestNonSiteTravelOptionResponse>>;
}
