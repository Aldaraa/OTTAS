using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.RosterFeature.CreateRoster
{
    public sealed class CreateRosterValidator : AbstractValidator<CreateRosterRequest>
    {
        public CreateRosterValidator()
        {
            RuleFor(x => x.Name).NotEmpty().MaximumLength(50);
            RuleFor(x => x.Description).NotEmpty();
            RuleFor(x => x.RosterGroupId).NotEmpty();

        }
    }
}
