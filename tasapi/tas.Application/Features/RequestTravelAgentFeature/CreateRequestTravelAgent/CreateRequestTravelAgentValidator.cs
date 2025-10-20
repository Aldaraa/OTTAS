using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.RequestTravelAgentFeature.CreateRequestTravelAgent
{
    public sealed class CreateRequestTravelAgentValidator : AbstractValidator<CreateRequestTravelAgentRequest>
    {
        public CreateRequestTravelAgentValidator()
        {
            RuleFor(x => x.Description).NotEmpty();
        }
    }
}
