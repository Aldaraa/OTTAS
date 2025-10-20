using MediatR;

namespace tas.Application.Features.RequestDocumentAttachmentFeature.GetRequestDocumentAttachment
{ 
    public sealed record GetRequestDocumentAttachmentRequest(int DocumentId) : IRequest<List<GetRequestDocumentAttachmentResponse>>;
}
