using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.RequestDocumentAttachmentFeature.UpdateRequestDocumentAttachment 
{ 
    public sealed class UpdateRequestDocumentAttachmentValidator : AbstractValidator<UpdateRequestDocumentAttachmentRequest>
    {
        public UpdateRequestDocumentAttachmentValidator()
        {
            RuleFor(request => request.Id).NotEmpty().WithMessage("Id is required.");
        }
    }
}
