using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.RequestDocumentAttachmentFeature.CreateRequestDocumentAttachment
{
    public sealed record CreateRequestDocumentAttachmentRequest(
        List<int> FileAddressIds,
        string? Description,
        int IncludeEmail,
        int DocumentId

        ) : IRequest;
}
