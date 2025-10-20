using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.SysTeamFeature.SetMenuSysTeam;

namespace tas.Application.Features.RequestDocumentExternalTravelFeature.UpdateRequestDocumentExternalTravelAdd
{

    public class UpdateRequestDocumentExternalTravelAddValidator : AbstractValidator<UpdateRequestDocumentExternalTravelAddRequest>
    {
        public UpdateRequestDocumentExternalTravelAddValidator()
        {
            RuleFor(data => data.Id).NotEmpty();
            RuleFor(data => data.FirstScheduleId).NotEmpty();
            RuleFor(data => data.departmentId).NotEmpty();
            RuleFor(data => data.employerId).NotEmpty();
            RuleFor(data => data.positionId).NotEmpty();
            RuleFor(data => data.costcodeId).NotEmpty();
        }


    }





}
