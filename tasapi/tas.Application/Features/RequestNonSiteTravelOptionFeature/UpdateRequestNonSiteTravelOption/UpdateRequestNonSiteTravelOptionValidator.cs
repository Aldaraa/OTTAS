using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.RequestTravelAgentFeature.UpdateRequestTravelAgent;

namespace tas.Application.Features.RequestNonSiteTravelOptionFeature.UpdateRequestNonSiteTravelOption
{
    public sealed class UpdateRequestNonSiteTravelOptionValidator : AbstractValidator<UpdateRequestNonSiteTravelOptionRequest>
    {
        public UpdateRequestNonSiteTravelOptionValidator()
        {
            RuleFor(x => x.Id).NotEmpty();
            RuleFor(x => x.Selected).NotEmpty();
        }
    }
}
