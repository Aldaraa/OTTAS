using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace tas.Application.Features.RequestDocumentExternalTravelFeature.CreateRequestDocumentExternalTravelRemove
{

    public class CreateRequestDocumentExternalTravelRemoveValidator : AbstractValidator<CreateRequestDocumentExternalTravelRemoveRequest>
    {
        public CreateRequestDocumentExternalTravelRemoveValidator()
        {
            RuleFor(request => request.documentData).SetValidator(new CreateRequestDocumentExternalTravelRemoveDocumentValidator());
            RuleFor(request => request.flightData).SetValidator(new CreateRequestDocumentExternalTravelRemoveDataValidator());
            RuleForEach(request => request.Files).SetValidator(new CreateRequestDocumentExternalTravelRemoveAttachmentValidator());
        }


    }


    public class CreateRequestDocumentExternalTravelRemoveDocumentValidator : AbstractValidator<CreateRequestDocumentExternalTravelRemoveDocument>
    {
        public CreateRequestDocumentExternalTravelRemoveDocumentValidator()
        {
            RuleFor(document => document.EmployeeId).NotEmpty();
            RuleFor(document => document.Action).NotEmpty();
            RuleFor(document => document.NextGroupId).NotEmpty();
        }
    }

    public class CreateRequestDocumentExternalTravelRemoveDataValidator : AbstractValidator<CreateRequestDocumentExternalTravelRemoveData>
    {
        public CreateRequestDocumentExternalTravelRemoveDataValidator()
        {
            RuleFor(data => data.TransportId).NotEmpty();
        }
    }

    public class CreateRequestDocumentExternalTravelRemoveAttachmentValidator : AbstractValidator<CreateRequestDocumentExternalTravelRemoveAttachment>
    {
        public CreateRequestDocumentExternalTravelRemoveAttachmentValidator()
        {
            RuleFor(attachment => attachment.FileAddressId).NotEmpty();
            RuleFor(attachment => attachment.IncludeEmail).NotEmpty();
        }
    }



}
