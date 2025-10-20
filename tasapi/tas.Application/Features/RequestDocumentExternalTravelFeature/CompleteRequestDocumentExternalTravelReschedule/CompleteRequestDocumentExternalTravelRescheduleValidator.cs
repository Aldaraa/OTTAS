using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.SysTeamFeature.SetMenuSysTeam;

namespace tas.Application.Features.RequestDocumentExternalTravelFeature.CompleteRequestDocumentExternalTravelReschedule
{

    public class CompleteRequestDocumentExternalTravelRescheduleValidator : AbstractValidator<CompleteRequestDocumentExternalTravelRescheduleRequest>
    {
        public CompleteRequestDocumentExternalTravelRescheduleValidator()
        {
            RuleFor(request => request.documentId).NotEmpty();
        }

    }





}
