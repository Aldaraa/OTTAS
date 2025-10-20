using MediatR;

namespace tas.Application.Features.RequestNonSiteTicketConfigFeature.GetAllRequestNonSiteTicketConfig
{
    public sealed record GetAllRequestNonSiteTicketConfigRequest(int? status) : IRequest<List<GetAllRequestNonSiteTicketConfigResponse>>;
}
