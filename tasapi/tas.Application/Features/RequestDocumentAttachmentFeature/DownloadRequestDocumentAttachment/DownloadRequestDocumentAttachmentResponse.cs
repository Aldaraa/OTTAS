
using MediatR;
using tas.Domain.Entities;

namespace tas.Application.Features.RequestDocumentAttachmentFeature.DownloadRequestDocumentAttachment
{
    public sealed record DownloadRequestDocumentAttachmentResponse
    {
        public List<string> Attachments { get; set; }

    }



}