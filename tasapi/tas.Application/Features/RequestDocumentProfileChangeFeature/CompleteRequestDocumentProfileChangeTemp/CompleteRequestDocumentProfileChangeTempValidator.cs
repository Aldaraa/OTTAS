using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.RequestDocumentProfileChangeFeature.CompleteRequestDocumentProfileChangeTemp;
using tas.Application.Features.SysTeamFeature.SetMenuSysTeam;

namespace tas.Application.Features.RequestDocumentProfileChangeFeature.CompleteRequestDocumentProfileChangeTemp
{


    public class CompleteRequestDocumentProfileChangeTempValidator : AbstractValidator<CompleteRequestDocumentProfileChangeTempRequest>
    {
        public CompleteRequestDocumentProfileChangeTempValidator()
        {
            RuleFor(x => x.documentId).NotNull();
        
            //RuleFor(x => x.RequestInfo).NotNull().SetValidator(new CreateRequestDocumentNonSiteTravelRequestInfoValidator());
        }
    }





}
