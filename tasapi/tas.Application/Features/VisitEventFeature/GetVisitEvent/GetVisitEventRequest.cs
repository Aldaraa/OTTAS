using MediatR;

namespace tas.Application.Features.VisitEventFeature.GetVisitEvent
{
    public sealed record GetVisitEventRequest(int Id) : IRequest<GetVisitEventResponse>;
}
