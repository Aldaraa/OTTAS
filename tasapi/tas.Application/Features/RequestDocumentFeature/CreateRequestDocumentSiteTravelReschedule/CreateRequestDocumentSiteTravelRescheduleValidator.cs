using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.RequestDocumentFeature.DeclineRequestDocument;
using tas.Application.Features.SysTeamFeature.SetMenuSysTeam;

namespace tas.Application.Features.RequestDocumentFeature.CreateRequestDocumentSiteTravelReschedule
{

    public class CreateRequestDocumentSiteTravelRescheduleValidator : AbstractValidator<CreateRequestDocumentSiteTravelRescheduleRequest>
    {
        public CreateRequestDocumentSiteTravelRescheduleValidator()
        {
            RuleFor(request => request.documentData).SetValidator(new CreateRequestDocumentSiteTravelRescheduleDocumentValidator()) ;
            RuleFor(request => request.flightData).SetValidator(new CreateRequestDocumentSiteTravelRescheduleDataValidator());
            RuleForEach(request => request.Files).SetValidator(new CreateRequestDocumentSiteTravelRescheduleAttachmentValidator());
        }

    }


    public class CreateRequestDocumentSiteTravelRescheduleDocumentValidator : AbstractValidator<CreateRequestDocumentSiteTravelRescheduleDocument>
    {
        public CreateRequestDocumentSiteTravelRescheduleDocumentValidator()
        {
            RuleFor(document => document.EmployeeId).NotEmpty();
            RuleFor(document => document.Action).NotEmpty();
            RuleFor(document => document.NextGroupId).NotEmpty();
        }
    }

    public class CreateRequestDocumentSiteTravelRescheduleDataValidator : AbstractValidator<CreateRequestDocumentSiteTravelRescheduleData>
    {
        public CreateRequestDocumentSiteTravelRescheduleDataValidator()
        {
            RuleFor(data => data.existingScheduleId).NotEmpty();
            RuleFor(data => data.reScheduleId).NotEmpty();
            RuleFor(data => data.shiftId).NotEmpty();
        }
    }

    public class CreateRequestDocumentSiteTravelRescheduleAttachmentValidator : AbstractValidator<CreateRequestDocumentSiteTravelRescheduleAttachment>
    {
        public CreateRequestDocumentSiteTravelRescheduleAttachmentValidator()
        {
            RuleFor(attachment => attachment.FileAddressId).NotEmpty();
            RuleFor(attachment => attachment.IncludeEmail).NotEmpty();
        }
    }



}
