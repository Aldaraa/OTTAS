using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.SysTeamFeature.SetMenuSysTeam;

namespace tas.Application.Features.RequestDocumentFeature.ChangeLineManagerRequestDocument
{


    public class ChangeLineManagerRequestDocumentRequestValidator : AbstractValidator<ChangeLineManagerRequestDocumentRequest>
    {
        public ChangeLineManagerRequestDocumentRequestValidator()
        {
            RuleFor(x => x.Id).NotNull();
            RuleFor(x => x.newAssignEmployeeId).NotNull();
           
        }
    }




}
