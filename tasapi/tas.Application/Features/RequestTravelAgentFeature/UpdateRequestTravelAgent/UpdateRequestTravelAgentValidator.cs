using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.RequestTravelAgentFeature.UpdateRequestTravelAgent;

namespace tas.Application.Features.RequestTravelAgenteFeature.UpdateRequestTravelAgent
{
    public sealed class UpdateRequestTravelAgentValidator : AbstractValidator<UpdateRequestTravelAgentRequest>
    {
        public UpdateRequestTravelAgentValidator()
        {
            RuleFor(x => x.Id).NotEmpty();
            RuleFor(x => x.Description).NotEmpty();
        }
    }
}
