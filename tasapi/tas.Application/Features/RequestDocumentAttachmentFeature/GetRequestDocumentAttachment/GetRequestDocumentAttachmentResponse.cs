
using MediatR;
using tas.Domain.Entities;

namespace tas.Application.Features.RequestDocumentAttachmentFeature.GetRequestDocumentAttachment
{
    public sealed record GetRequestDocumentAttachmentResponse
    {
        public int Id { get; set; }
        
        public string? FileAddress { get; set; }

        public int? IncludeEmail { get; set; }

        public string? Description { get; set; }

        public int DocumentId { get; set; }

        public string? FullName { get; set; }

        public DateTime? CreatedDate { get; set; }

    }



}