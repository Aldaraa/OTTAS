using MediatR;

namespace tas.Application.Features.RequestDocumentExternalTravelFeature.GetRequestDocumentExternalTravelAdd
{
    public sealed record GetRequestDocumentExternalTravelAddRequest(int documentId) : IRequest<GetRequestDocumentExternalTravelAddResponse>;
}
