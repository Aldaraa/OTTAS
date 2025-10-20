using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.RosterFeature.UpdateRoster;

namespace tas.Application.Features.RostereFeature.UpdateRostere
{
    public sealed class UpdateRosterValidator : AbstractValidator<UpdateRosterRequest>
    {
        public UpdateRosterValidator()
        {
            RuleFor(x => x.Id).NotEmpty();
            RuleFor(x => x.Description).NotEmpty().MaximumLength(50);
            RuleFor(x => x.Name).NotEmpty();
            RuleFor(x => x.RosterGroupId).NotEmpty();
        }
    }
}
