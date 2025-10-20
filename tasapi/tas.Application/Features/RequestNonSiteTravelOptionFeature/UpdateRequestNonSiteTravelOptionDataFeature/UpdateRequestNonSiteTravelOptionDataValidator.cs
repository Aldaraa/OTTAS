using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.RequestTravelAgentFeature.UpdateRequestTravelAgent;

namespace tas.Application.Features.RequestNonSiteTravelOptionDataFeature.UpdateRequestNonSiteTravelOptionData
{
    public sealed class UpdateRequestNonSiteTravelOptionDataValidator : AbstractValidator<UpdateRequestNonSiteTravelOptionDataRequest>
    {
        public UpdateRequestNonSiteTravelOptionDataValidator()
        {
            RuleFor(x => x.Id).NotEmpty();
            RuleFor(x => x.DueDate).NotEmpty();
            RuleFor(x => x.optionData).NotEmpty();
            RuleFor(x => x.Cost).NotEmpty();

        }
    }
}
