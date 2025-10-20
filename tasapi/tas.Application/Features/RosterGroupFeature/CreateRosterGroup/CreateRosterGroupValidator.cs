using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.RosterGroupFeature.CreateRosterGroup
{
    public sealed class CreateRosterGroupValidator : AbstractValidator<CreateRosterGroupRequest>
    {
        public CreateRosterGroupValidator()
        {
            RuleFor(x => x.Description).NotEmpty().MaximumLength(50);
        }
    }
}
