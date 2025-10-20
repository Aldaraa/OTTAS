using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.RequestDocumentFeature.DeclineRequestDocument;
using tas.Application.Features.SysTeamFeature.SetMenuSysTeam;

namespace tas.Application.Features.RequestDocumentFeature.CreateRequestDocumentSiteTravelRemove
{

    public class CreateRequestDocumentSiteTravelRemoveValidator : AbstractValidator<CreateRequestDocumentSiteTravelRemoveRequest>
    {
        public CreateRequestDocumentSiteTravelRemoveValidator()
        {
            RuleFor(request => request.documentData).SetValidator(new CreateRequestDocumentSiteTravelRemoveDocumentValidator());
            RuleFor(request => request.flightData).SetValidator(new CreateRequestDocumentSiteTravelRemoveDataValidator());
            RuleForEach(request => request.Files).SetValidator(new CreateRequestDocumentSiteTravelRemoveAttachmentValidator());
        }


    }


    public class CreateRequestDocumentSiteTravelRemoveDocumentValidator : AbstractValidator<CreateRequestDocumentSiteTravelRemoveDocument>
    {
        public CreateRequestDocumentSiteTravelRemoveDocumentValidator()
        {
            RuleFor(document => document.EmployeeId).NotEmpty();
            RuleFor(document => document.Action).NotEmpty();
            RuleFor(document => document.NextGroupId).NotEmpty();
        }
    }

    public class CreateRequestDocumentSiteTravelRemoveDataValidator : AbstractValidator<CreateRequestDocumentSiteTravelRemoveData>
    {
        public CreateRequestDocumentSiteTravelRemoveDataValidator()
        {
            RuleFor(data => data.FirstScheduleId).NotEmpty();
            RuleFor(data => data.LastScheduleId).NotEmpty();
            RuleFor(data => data.shiftId).NotEmpty();
        }
    }

    public class CreateRequestDocumentSiteTravelRemoveAttachmentValidator : AbstractValidator<CreateRequestDocumentSiteTravelRemoveAttachment>
    {
        public CreateRequestDocumentSiteTravelRemoveAttachmentValidator()
        {
            RuleFor(attachment => attachment.FileAddressId).NotEmpty();
            RuleFor(attachment => attachment.IncludeEmail).NotEmpty();
        }
    }



}
