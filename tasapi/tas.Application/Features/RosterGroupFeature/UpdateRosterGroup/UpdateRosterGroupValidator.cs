using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.RosterGroupFeature.UpdateRosterGroup;

namespace tas.Application.Features.RosterGroupeFeature.UpdateRosterGroupe
{
    public sealed class UpdateRosterGroupValidator : AbstractValidator<UpdateRosterGroupRequest>
    {
        public UpdateRosterGroupValidator()
        {
            RuleFor(x => x.Id).NotEmpty();
            RuleFor(x => x.Description).NotEmpty();
        }
    }
}
