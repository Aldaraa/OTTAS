using MediatR;

namespace tas.Application.Features.RequestDocumentFeature.GetNonSiteTravelMaster
{
    public sealed record GetNonSiteTravelMasterRequest : IRequest<GetNonSiteTravelMasterResponse>;
}
