using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.SysTeamFeature.SetMenuSysTeam;

namespace tas.Application.Features.RequestDocumentExternalTravelFeature.CompleteRequestDocumentExternalTravelAdd
{

    public class CompleteRequestDocumentExternalTravelAddValidator : AbstractValidator<CompleteRequestDocumentExternalTravelAddRequest>
    {
        public CompleteRequestDocumentExternalTravelAddValidator()
        {
            RuleFor(request => request.documentId).NotEmpty();

        }


    }




}
