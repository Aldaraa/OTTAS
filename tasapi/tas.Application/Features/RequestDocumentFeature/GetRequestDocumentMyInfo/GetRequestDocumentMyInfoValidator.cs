using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.SysTeamFeature.SetMenuSysTeam;

namespace tas.Application.Features.RequestDocumentFeature.GetRequestDocumentNonSiteTravel
{


    public class GetRequestDocumentNonSiteTravelRequestValidator : AbstractValidator<GetRequestDocumentNonSiteTravelRequest>
    {
        public GetRequestDocumentNonSiteTravelRequestValidator()
        {
            RuleFor(x => x.documentId).NotNull();
        }
    }






}
