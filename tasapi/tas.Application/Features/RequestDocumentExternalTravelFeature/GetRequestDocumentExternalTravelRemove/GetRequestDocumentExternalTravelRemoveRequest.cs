using MediatR;

namespace tas.Application.Features.RequestDocumentExternalTravelFeature.GetRequestDocumentExternalTravelRemove
{
    public sealed record GetRequestDocumentExternalTravelRemoveRequest(int documentId) : IRequest<GetRequestDocumentExternalTravelRemoveResponse>;
}
