using MediatR;

namespace tas.Application.Features.RequestDocumentFeature.GetRequestDocumentSiteTravelRemove
{
    public sealed record GetRequestDocumentSiteTravelRemoveRequest(int documentId) : IRequest<GetRequestDocumentSiteTravelRemoveResponse>;
}
