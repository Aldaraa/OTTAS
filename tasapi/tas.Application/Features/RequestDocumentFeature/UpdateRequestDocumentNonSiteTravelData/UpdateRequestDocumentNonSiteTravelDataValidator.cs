using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.RequestDocumentFeature.UpdateRequestDocumentNonSiteTravelData;
using tas.Application.Features.SysTeamFeature.SetMenuSysTeam;

namespace tas.Application.Features.RequestDocumentFeature.CreateRequestDocumentNonSiteTravel
{


    public class UpdateRequestDocumentNonSiteTravelDataValidator : AbstractValidator<UpdateRequestDocumentNonSiteTravelDataRequest>
    {
        public UpdateRequestDocumentNonSiteTravelDataValidator()
        {
            RuleFor(x => x.DocumentId).NotEmpty();
            RuleFor(x => x.RequestTravelAgentId).NotEmpty();
            RuleFor(x => x.RequestTravelAgentSureName).NotEmpty().MaximumLength(50);
            RuleFor(x => x.DocumentId).NotEmpty();


            //RuleFor(x => x.RequestInfo).NotNull().SetValidator(new UpdateRequestDocumentNonSiteTravelDataInfoValidator());
        }
    }




}
