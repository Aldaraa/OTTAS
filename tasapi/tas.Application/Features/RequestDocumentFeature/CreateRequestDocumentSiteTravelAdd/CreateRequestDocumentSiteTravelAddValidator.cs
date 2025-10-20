using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.RequestDocumentFeature.DeclineRequestDocument;
using tas.Application.Features.SysTeamFeature.SetMenuSysTeam;

namespace tas.Application.Features.RequestDocumentFeature.CreateRequestDocumentSiteTravelAdd
{

    public class CreateRequestDocumentSiteTravelAddValidator : AbstractValidator<CreateRequestDocumentSiteTravelAddRequest>
    {
        public CreateRequestDocumentSiteTravelAddValidator()
        {
            RuleFor(request => request.documentData).SetValidator(new CreateRequestDocumentSiteTravelDocumentValidator());
            RuleFor(request => request.flightData).SetValidator(new CreateRequestDocumentSiteTravelDataValidator());
            RuleForEach(request => request.Files).SetValidator(new CreateRequestDocumentSiteTravelAttachmentValidator());
        }


    }


    public class CreateRequestDocumentSiteTravelDocumentValidator : AbstractValidator<CreateRequestDocumentSiteTravelDocument>
    {
        public CreateRequestDocumentSiteTravelDocumentValidator()
        {
            RuleFor(document => document.EmployeeId).NotEmpty();
            RuleFor(document => document.Action).NotEmpty();
            RuleFor(document => document.NextGroupId).NotEmpty();
        }
    }

    public class CreateRequestDocumentSiteTravelDataValidator : AbstractValidator<CreateRequestDocumentSiteTravelData>
    {
        public CreateRequestDocumentSiteTravelDataValidator()
        {
            RuleFor(data => data.inScheduleId).NotEmpty();
            RuleFor(data => data.outScheduleId).NotEmpty();
            RuleFor(data => data.departmentId).NotEmpty();
            RuleFor(data => data.shiftId).NotEmpty();
            RuleFor(data => data.employerId).NotEmpty();
            RuleFor(data => data.positionId).NotEmpty();
            RuleFor(data => data.costcodeId).NotEmpty();
        }
    }

    public class CreateRequestDocumentSiteTravelAttachmentValidator : AbstractValidator<CreateRequestDocumentSiteTravelAttachment>
    {
        public CreateRequestDocumentSiteTravelAttachmentValidator()
        {
            RuleFor(attachment => attachment.FileAddressId).NotEmpty();
            RuleFor(attachment => attachment.IncludeEmail).NotEmpty();
        }
    }



}
