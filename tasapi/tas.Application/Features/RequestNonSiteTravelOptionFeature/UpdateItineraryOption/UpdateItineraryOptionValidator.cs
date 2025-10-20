using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.RequestTravelAgentFeature.UpdateRequestTravelAgent;

namespace tas.Application.Features.RequestNonSiteTravelOptionFeature.UpdateItineraryOption
{
    public sealed class UpdateItineraryOptionValidator : AbstractValidator<UpdateItineraryOptionRequest>
    {
        public UpdateItineraryOptionValidator()
        {
            RuleFor(x => x.DocumentId).NotEmpty();
            RuleFor(x => x.optionText).NotEmpty();

        }
    }
}
