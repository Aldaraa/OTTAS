using MediatR;

namespace tas.Application.Features.RequestDocumentExternalTravelFeature.GetRequestDocumentExternalTravelReschedule
{
    public sealed record GetRequestDocumentExternalTravelRescheduleRequest(int documentId) : IRequest<GetRequestDocumentExternalTravelRescheduleResponse>;
}
