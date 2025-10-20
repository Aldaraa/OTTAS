using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.RequestDocumentExternalTravelFeature.CreateRequestDocumentExternalTravelReschedule
{

    public class CreateRequestDocumentExternalTravelRescheduleValidator : AbstractValidator<CreateRequestDocumentExternalTravelRescheduleRequest>
    {
        public CreateRequestDocumentExternalTravelRescheduleValidator()
        {
            RuleFor(request => request.documentData).SetValidator(new CreateRequestDocumentExternalTravelRescheduleDocumentValidator()) ;
            RuleFor(request => request.flightData).SetValidator(new CreateRequestDocumentExternalTravelRescheduleDataValidator());
            RuleForEach(request => request.Files).SetValidator(new CreateRequestDocumentExternalTravelRescheduleAttachmentValidator());
        }

    }


    public class CreateRequestDocumentExternalTravelRescheduleDocumentValidator : AbstractValidator<CreateRequestDocumentExternalTravelRescheduleDocument>
    {
        public CreateRequestDocumentExternalTravelRescheduleDocumentValidator()
        {
            RuleFor(document => document.EmployeeId).NotEmpty();
            RuleFor(document => document.Action).NotEmpty();
            RuleFor(document => document.NextGroupId).NotEmpty();
        }
    }

    public class CreateRequestDocumentExternalTravelRescheduleDataValidator : AbstractValidator<CreateRequestDocumentExternalTravelRescheduleData>
    {
        public CreateRequestDocumentExternalTravelRescheduleDataValidator()
        {
            RuleFor(data => data.ScheduleId).NotEmpty();
            RuleFor(data => data.oldTransportId).NotEmpty();
        }
    }

    public class CreateRequestDocumentExternalTravelRescheduleAttachmentValidator : AbstractValidator<CreateRequestDocumentExternalTravelRescheduleAttachment>
    {
        public CreateRequestDocumentExternalTravelRescheduleAttachmentValidator()
        {
            RuleFor(attachment => attachment.FileAddressId).NotEmpty();
            RuleFor(attachment => attachment.IncludeEmail).NotEmpty();
        }
    }



}
