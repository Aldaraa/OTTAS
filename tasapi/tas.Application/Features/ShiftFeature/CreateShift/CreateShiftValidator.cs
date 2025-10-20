using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.ShiftFeature.CreateShift
{
    public sealed class CreateShiftValidator : AbstractValidator<CreateShiftRequest>
    {
        public CreateShiftValidator()
        {
            RuleFor(x => x.Code).NotEmpty().MaximumLength(150);
            RuleFor(x => x.Description).NotEmpty();
            RuleFor(x => x.Active)
             .Must(value => value == 0 || value == 1)
             .WithMessage("Active must be either 0 or 1.");
        }
    }
}
