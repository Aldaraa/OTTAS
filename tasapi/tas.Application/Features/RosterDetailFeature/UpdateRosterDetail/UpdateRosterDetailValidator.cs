using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.RosterDetailFeature.UpdateRosterDetail;

namespace tas.Application.Features.RosterDetaileFeature.UpdateRosterDetaile
{
    public sealed class UpdateRosterDetailValidator : AbstractValidator<UpdateRosterDetailRequest>
    {
        public UpdateRosterDetailValidator()
        {
            RuleFor(x => x.DaysOn).NotEmpty().GreaterThan(0);
            RuleFor(x => x.ShiftId).NotEmpty().GreaterThan(0);
            RuleFor(x => x.Rosterid).NotEmpty().GreaterThan(0);
            RuleFor(x => x.Id).NotEmpty().GreaterThan(0);
            RuleFor(x => x.Active)
             .Must(value => value == 0 || value == 1)
             .WithMessage("Active must be either 0 or 1.");


        }
    }
}
