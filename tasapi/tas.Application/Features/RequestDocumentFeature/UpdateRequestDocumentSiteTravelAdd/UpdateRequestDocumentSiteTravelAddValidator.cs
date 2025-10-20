using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.RequestDocumentFeature.DeclineRequestDocument;
using tas.Application.Features.SysTeamFeature.SetMenuSysTeam;

namespace tas.Application.Features.RequestDocumentFeature.UpdateRequestDocumentSiteTravelAdd
{

    public class UpdateRequestDocumentSiteTravelAddValidator : AbstractValidator<UpdateRequestDocumentSiteTravelAddRequest>
    {
        public UpdateRequestDocumentSiteTravelAddValidator()
        {
            RuleFor(data => data.Id).NotEmpty();
            RuleFor(data => data.inScheduleId).NotEmpty();
            RuleFor(data => data.outScheduleId).NotEmpty();
            RuleFor(data => data.departmentId).NotEmpty();
            RuleFor(data => data.shiftId).NotEmpty();
            RuleFor(data => data.employerId).NotEmpty();
            RuleFor(data => data.positionId).NotEmpty();
            RuleFor(data => data.costcodeId).NotEmpty();
        }


    }





}
