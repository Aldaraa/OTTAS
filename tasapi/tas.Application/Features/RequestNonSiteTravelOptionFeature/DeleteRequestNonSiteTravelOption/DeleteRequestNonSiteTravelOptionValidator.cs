using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.RequestTravelAgentFeature.UpdateRequestTravelAgent;

namespace tas.Application.Features.RequestNonSiteTravelOptionFeature.DeleteRequestNonSiteTravelOption
{
    public sealed class DeleteRequestNonSiteTravelOptionValidator : AbstractValidator<DeleteRequestNonSiteTravelOptionRequest>
    {
        public DeleteRequestNonSiteTravelOptionValidator()
        {
            RuleFor(x => x.Id).NotEmpty();
        }
    }
}
