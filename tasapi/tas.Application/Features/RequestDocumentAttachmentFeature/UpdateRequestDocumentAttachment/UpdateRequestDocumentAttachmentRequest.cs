using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.RequestDocumentAttachmentFeature.UpdateRequestDocumentAttachment
{
    public sealed record UpdateRequestDocumentAttachmentRequest(
        string? Description,
        int IncludeEmail,
        int Id  

        ) : IRequest<int>;
}
