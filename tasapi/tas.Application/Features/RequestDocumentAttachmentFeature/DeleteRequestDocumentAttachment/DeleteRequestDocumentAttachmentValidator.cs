using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.RequestDocumentAttachmentFeature.DeleteRequestDocumentAttachment 
{ 
    public sealed class DeleteRequestDocumentAttachmentValidator : AbstractValidator<DeleteRequestDocumentAttachmentRequest>
    {
        public DeleteRequestDocumentAttachmentValidator()
        {
            RuleFor(x => x.Id).NotEmpty();
        }
    }
}
