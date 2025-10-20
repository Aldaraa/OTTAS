using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.TransportModeFeature.CreateTransportMode
{
    public sealed class CreateTransportModeValidator : AbstractValidator<CreateTransportModeRequest>
    {
        public CreateTransportModeValidator()
        {
            RuleFor(x => x.Code).NotEmpty().MaximumLength(20);
            RuleFor(x => x.Active)
             .Must(value => value == 0 || value == 1)
             .WithMessage("Active must be either 0 or 1.");
        }
    }
}
