using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.RequestDocumentDeMobilisationFeature.UpdateRequestDocumentDeMobilisation;
using tas.Application.Features.SysTeamFeature.SetMenuSysTeam;

namespace tas.Application.Features.RequestDocumentDeMobilisationFeature.UpdateRequestDocumentDeMobilisation
{


    public class UpdateRequestDocumentDeMobilisationValidator : AbstractValidator<UpdateRequestDocumentDeMobilisationRequest>
    {
        public UpdateRequestDocumentDeMobilisationValidator()
        {
            RuleFor(x => x.Id).NotNull();
            RuleFor(x => x.EmployerId).NotNull();
            RuleFor(x => x.CompletionDate).NotNull();



        }
    }





}
