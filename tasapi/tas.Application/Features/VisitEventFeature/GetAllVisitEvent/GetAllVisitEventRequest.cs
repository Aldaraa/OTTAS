using MediatR;
using tas.Application.Features.PositionFeature.GetAllPosition;

namespace tas.Application.Features.VisitEventFeature.GetAllVisitEvent
{
    public sealed record GetAllVisitEventRequest(DateTime? startDate, DateTime? endDate, string? Name) : IRequest<List<GetAllVisitEventResponse>>;
}
