using MediatR;

namespace tas.Application.Features.RequestDocumentFeature.GetRequestDocumentSiteTravelAdd
{
    public sealed record GetRequestDocumentSiteTravelAddRequest(int documentId) : IRequest<GetRequestDocumentSiteTravelAddResponse>;
}
