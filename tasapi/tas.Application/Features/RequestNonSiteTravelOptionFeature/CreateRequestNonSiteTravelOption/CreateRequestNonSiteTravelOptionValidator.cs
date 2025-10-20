using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.RequestTravelAgentFeature.UpdateRequestTravelAgent;

namespace tas.Application.Features.RequestNonSiteTravelOptionFeature.CreateRequestNonSiteTravelOption
{
    public sealed class CreateRequestNonSiteTravelOptionValidator : AbstractValidator<CreateRequestNonSiteTravelOptionRequest>
    {
        public CreateRequestNonSiteTravelOptionValidator()
        {
            RuleFor(x => x.DocumentId).NotEmpty();
            RuleFor(x => x.optionData).NotEmpty();
        }
    }
}
