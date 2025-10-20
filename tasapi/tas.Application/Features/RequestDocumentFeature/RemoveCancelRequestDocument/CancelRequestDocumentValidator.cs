using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.SysTeamFeature.SetMenuSysTeam;

namespace tas.Application.Features.RequestDocumentFeature.RemoveCancelRequestDocument
{


    public class RemoveCancelRequestDocumentRequestValidator : AbstractValidator<RemoveCancelRequestDocumentRequest>
    {
        public RemoveCancelRequestDocumentRequestValidator()
        {
            RuleFor(x => x.documentId).NotNull();
           
        }
    }




}
