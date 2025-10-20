using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.RequestTravelPurposeFeature.CreateRequestTravelPurpose
{
    public sealed class CreateRequestTravelPurposeValidator : AbstractValidator<CreateRequestTravelPurposeRequest>
    {
        public CreateRequestTravelPurposeValidator()
        {
            RuleFor(x => x.Code).NotEmpty().MaximumLength(150);
            RuleFor(x => x.Description).NotEmpty();
        }
    }
}
