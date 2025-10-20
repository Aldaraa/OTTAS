using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.RequestDocumentFeature.DeclineRequestDocument;
using tas.Application.Features.SysTeamFeature.SetMenuSysTeam;

namespace tas.Application.Features.RequestDocumentFeature.CompleteRequestDocumentSiteTravelReschedule
{

    public class CompleteRequestDocumentSiteTravelRescheduleValidator : AbstractValidator<CompleteRequestDocumentSiteTravelRescheduleRequest>
    {
        public CompleteRequestDocumentSiteTravelRescheduleValidator()
        {
            RuleFor(request => request.Id).NotEmpty();
        }

    }





}
