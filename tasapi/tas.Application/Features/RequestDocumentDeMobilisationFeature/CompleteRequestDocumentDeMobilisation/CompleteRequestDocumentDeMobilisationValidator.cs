using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.SysTeamFeature.SetMenuSysTeam;

namespace tas.Application.Features.RequestDocumentDeMobilisationFeature.CompleteRequestDocumentDeMobilisation
{


    public class CompleteRequestDocumentDeMobilisationValidator : AbstractValidator<CompleteRequestDocumentDeMobilisationRequest>
    {
        public CompleteRequestDocumentDeMobilisationValidator()
        {
           RuleFor(x => x.documentId).NotNull();
        
            //RuleFor(x => x.RequestInfo).NotNull().SetValidator(new CompleteRequestDocumentNonSiteTravelRequestInfoValidator());
        }
    }





}
