using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.SysTeamFeature.SetMenuSysTeam;

namespace tas.Application.Features.RequestDocumentDeMobilisationFeature.CreateRequestDocumentDeMobilisation
{


    public class CreateRequestDocumentDeMobilisationValidator : AbstractValidator<CreateRequestDocumentDeMobilisationRequest>
    {
        public CreateRequestDocumentDeMobilisationValidator()
        {
         //.   RuleFor(x => x.travelData).NotNull();
        
            //RuleFor(x => x.RequestInfo).NotNull().SetValidator(new CreateRequestDocumentNonSiteTravelRequestInfoValidator());
        }
    }





}
