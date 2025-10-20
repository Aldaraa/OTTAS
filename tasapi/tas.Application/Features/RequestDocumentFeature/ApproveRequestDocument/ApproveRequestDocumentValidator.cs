using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.SysTeamFeature.SetMenuSysTeam;

namespace tas.Application.Features.RequestDocumentFeature.ApproveRequestDocument
{


    public class ApproveRequestDocumentRequestValidator : AbstractValidator<ApproveRequestDocumentRequest>
    {
        public ApproveRequestDocumentRequestValidator()
        {
            RuleFor(x => x.Id).NotNull();
           
        }
    }




}
