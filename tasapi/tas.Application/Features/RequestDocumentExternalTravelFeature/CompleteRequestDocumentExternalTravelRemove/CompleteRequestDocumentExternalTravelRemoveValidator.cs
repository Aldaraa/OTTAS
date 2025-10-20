using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.SysTeamFeature.SetMenuSysTeam;

namespace tas.Application.Features.RequestDocumentExternalTravelFeature.CompleteRequestDocumentExternalTravelRemove
{

    public class CompleteRequestDocumentExternalTravelRemoveValidator : AbstractValidator<CompleteRequestDocumentExternalTravelRemoveRequest>
    {
        public CompleteRequestDocumentExternalTravelRemoveValidator()
        {
            RuleFor(request => request.documentId).NotEmpty();
        }


    }



}
