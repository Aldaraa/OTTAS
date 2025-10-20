using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.RequestAirportFeature.CreateRequestAirport
{
    public sealed class CreateRequestAirportValidator : AbstractValidator<CreateRequestAirportRequest>
    {
        public CreateRequestAirportValidator()
        {
            RuleFor(x => x.Code).NotEmpty().MaximumLength(50);
            RuleFor(x => x.Description).NotEmpty().MaximumLength(150);
            RuleFor(x => x.Country).NotEmpty().MaximumLength(50);
        }
    }
}
