using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.RequestDocumentFeature.DeclineRequestDocument;
using tas.Application.Features.SysTeamFeature.SetMenuSysTeam;

namespace tas.Application.Features.RequestDocumentFeature.UpdateRequestDocumentSiteTravelRemove
{

    public class UpdateRequestDocumentSiteTravelRemoveValidator : AbstractValidator<UpdateRequestDocumentSiteTravelRemoveRequest>
    {
        public UpdateRequestDocumentSiteTravelRemoveValidator()
        {
            RuleFor(data => data.Id).NotEmpty();
            RuleFor(data => data.FirstScheduleId).NotEmpty();
            RuleFor(data => data.LastScheduleId).NotEmpty();
            RuleFor(data => data.shiftId).NotEmpty();
        }


    }





}
