using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.RequestTravelAgentFeature.DeleteRequestTravelAgent
{
    public sealed class DeleteRequestTravelAgentValidator : AbstractValidator<DeleteRequestTravelAgentRequest>
    {
        public DeleteRequestTravelAgentValidator()
        {
            RuleFor(x => x.Id).NotEmpty();
        }
    }
}
