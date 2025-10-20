using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.RosterDetailFeature.CreateRoster
{
    public sealed class CreateRosterDetailValidator : AbstractValidator<CreateRosterDetailRequest>
    {
        public CreateRosterDetailValidator()
        {
            RuleFor(x => x.RosterId).NotEmpty();
            RuleFor(x => x.DaysOn).NotEmpty();
            RuleFor(x => x.ShiftId).NotEmpty();
            RuleFor(x => x.Active)
             .Must(value => value == 0 || value == 1)
             .WithMessage("Active must be either 0 or 1.");
        }
    }
}
