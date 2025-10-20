using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.SysTeamFeature.SetMenuSysTeam;

namespace tas.Application.Features.RequestDocumentFeature.WaitingAgentRequestDocumentNonSiteTravel
{


    public class WaitingAgentRequestDocumentNonSiteTravelRequestValidator : AbstractValidator<WaitingAgentRequestDocumentNonSiteTravelRequest>
    {
        public WaitingAgentRequestDocumentNonSiteTravelRequestValidator()
        {
            RuleFor(x => x.documentId).NotNull();
        
        }
    }




}
