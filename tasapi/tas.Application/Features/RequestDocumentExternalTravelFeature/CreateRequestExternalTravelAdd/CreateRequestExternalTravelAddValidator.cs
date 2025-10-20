using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.SysTeamFeature.SetMenuSysTeam;

namespace tas.Application.Features.RequestDocumentExternalTravelFeature.CreateRequestExternalTravelAdd
{

    public class CreateRequestExternalTravelAddValidator : AbstractValidator<CreateRequestExternalTravelAddRequest>
    {
        public CreateRequestExternalTravelAddValidator()
        {
            RuleFor(request => request.documentData).SetValidator(new CreateRequestDocumentExternalTravelDocumentValidator());
            RuleFor(request => request.flightData).SetValidator(new CreateRequestDocumentExternalTravelDataValidator());
            RuleForEach(request => request.Files).SetValidator(new CreateRequestDocumentExternalTravelAttachmentValidator());
        }


    }


    public class CreateRequestDocumentExternalTravelDocumentValidator : AbstractValidator<CreateRequestDocumentExternalTravelDocument>
    {
        public CreateRequestDocumentExternalTravelDocumentValidator()
        {
            RuleFor(document => document.EmployeeId).NotEmpty();
            RuleFor(document => document.Action).NotEmpty();
            RuleFor(document => document.NextGroupId).NotEmpty();
        }
    }

    public class CreateRequestDocumentExternalTravelDataValidator : AbstractValidator<CreateRequestDocumentExternalTravelData>
    {
        public CreateRequestDocumentExternalTravelDataValidator()
        {
            RuleFor(data => data.FirstScheduleId).NotEmpty();
            RuleFor(data => data.departmentId).NotEmpty();
            RuleFor(data => data.employerId).NotEmpty();
            RuleFor(data => data.positionId).NotEmpty();
            RuleFor(data => data.costcodeId).NotEmpty();
        }
    }

    public class CreateRequestDocumentExternalTravelAttachmentValidator : AbstractValidator<CreateRequestDocumentExternalTravelAttachment>
    {
        public CreateRequestDocumentExternalTravelAttachmentValidator()
        {
            RuleFor(attachment => attachment.FileAddressId).NotEmpty();
            RuleFor(attachment => attachment.IncludeEmail).NotEmpty();
        }
    }



}
