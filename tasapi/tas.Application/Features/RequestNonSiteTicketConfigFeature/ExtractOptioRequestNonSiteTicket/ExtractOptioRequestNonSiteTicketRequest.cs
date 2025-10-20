using MediatR;

namespace tas.Application.Features.RequestNonSiteTicketConfigFeature.ExtractOptioRequestNonSiteTicket
{
    public sealed record ExtractOptioRequestNonSiteTicketRequest(string? OptionData) : IRequest<ExtractOptioRequestNonSiteTicketResponse>;
}
