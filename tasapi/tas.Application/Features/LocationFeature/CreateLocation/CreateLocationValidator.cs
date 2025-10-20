using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.LocationFeature.CreateLocation
{
    public sealed class CreateLocationValidator : AbstractValidator<CreateLocationRequest>
    {
        public CreateLocationValidator()
        {
            RuleFor(x => x.Code).NotEmpty().MaximumLength(150);
            RuleFor(x => x.onSite)
            .Must(value => value == 0 || value == 1)
            .WithMessage("Onsite must be either 0 or 1.");
        }
    }
}
