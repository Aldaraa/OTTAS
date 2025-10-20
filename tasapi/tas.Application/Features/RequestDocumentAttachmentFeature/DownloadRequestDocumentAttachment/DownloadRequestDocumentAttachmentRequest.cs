using MediatR;

namespace tas.Application.Features.RequestDocumentAttachmentFeature.DownloadRequestDocumentAttachment
{ 
    public sealed record DownloadRequestDocumentAttachmentRequest(int DocumentId) : IRequest<DownloadRequestDocumentAttachmentResponse>;
}
