using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.RequestDocumentFeature.DeclineRequestDocument;
using tas.Application.Features.SysTeamFeature.SetMenuSysTeam;

namespace tas.Application.Features.RequestDocumentFeature.UpdateRequestDocumentSiteTravelReschedule
{

    public class UpdateRequestDocumentSiteTravelRescheduleValidator : AbstractValidator<UpdateRequestDocumentSiteTravelRescheduleRequest>
    {
        public UpdateRequestDocumentSiteTravelRescheduleValidator()
        {
            RuleFor(data => data.Id).NotEmpty();
            RuleFor(data => data.ReScheduleId).NotEmpty();
            RuleFor(data => data.ExistingScheduleId).NotEmpty();
            RuleFor(data => data.shiftId).NotEmpty();
        }


    }





}
