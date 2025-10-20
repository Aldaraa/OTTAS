using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.SysTeamFeature.SetMenuSysTeam;

namespace tas.Application.Features.RequestDocumentExternalTravelFeature.UpdateRequestDocumentExternalTravelReschedule
{

    public class UpdateRequestDocumentExternalTravelRescheduleValidator : AbstractValidator<UpdateRequestDocumentExternalTravelRescheduleRequest>
    {
        public UpdateRequestDocumentExternalTravelRescheduleValidator()
        {
            RuleFor(data => data.Id).NotEmpty();
            RuleFor(data => data.newScheduleId).NotEmpty();
            RuleFor(data => data.CostCodeId).NotEmpty();
            RuleFor(data => data.DepartmentId).NotEmpty();

        }


    }





}
