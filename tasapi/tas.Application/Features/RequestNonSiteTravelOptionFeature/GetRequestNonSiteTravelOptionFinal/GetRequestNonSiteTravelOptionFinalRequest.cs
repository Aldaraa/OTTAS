using MediatR;

namespace tas.Application.Features.RequestNonSiteTravelOptionFeature.GetRequestNonSiteTravelOptionFinal
{
    public sealed record GetRequestNonSiteTravelOptionFinalRequest(int DocumentId) : IRequest<List<GetRequestNonSiteTravelOptionFinalResponse>>;
}
