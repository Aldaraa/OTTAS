using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.StateFeature.CreateState;

namespace tas.Application.Features.SysTeamFeature.SetMenuSysTeam
{
    public sealed class SetMenuSysTeamValidator : AbstractValidator<List<SetMenuSysTeamRequest>>
    {
        public SetMenuSysTeamValidator()
        {
            RuleForEach(x => x).ChildRules(request =>
            {
                request.RuleFor(x => x.MenuId).GreaterThan(0);
                request.RuleFor(x => x.TeamId).GreaterThan(0);
                request.RuleFor(x => x.Permission).Must(x => x == true || x == false).WithMessage("Permission must be either true or false.");
            });
        }
    }
}
