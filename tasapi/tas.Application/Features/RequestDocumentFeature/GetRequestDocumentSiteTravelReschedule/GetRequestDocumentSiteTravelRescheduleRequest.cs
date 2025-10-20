using MediatR;

namespace tas.Application.Features.RequestDocumentFeature.GetRequestDocumentSiteTravelReschedule
{
    public sealed record GetRequestDocumentSiteTravelRescheduleRequest(int documentId) : IRequest<GetRequestDocumentSiteTravelRescheduleResponse>;
}
