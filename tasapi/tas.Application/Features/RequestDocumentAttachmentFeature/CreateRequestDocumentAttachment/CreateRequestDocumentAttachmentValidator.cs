using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.RequestDocumentAttachmentFeature.CreateRequestDocumentAttachment 
{ 
    public sealed class CreateRequestDocumentAttachmentValidator : AbstractValidator<CreateRequestDocumentAttachmentRequest>
    {
        public CreateRequestDocumentAttachmentValidator()
        {
            RuleFor(request => request.FileAddressIds).NotEmpty().WithMessage("FileAddress is required.");
            RuleFor(request => request.DocumentId).NotEmpty().WithMessage("DocumentId is required.");
        }
    }
}
