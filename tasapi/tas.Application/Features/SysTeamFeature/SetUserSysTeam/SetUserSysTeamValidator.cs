using FluentValidation;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.StateFeature.CreateState;

namespace tas.Application.Features.SysTeamFeature.SetUserSysTeam
{
    public sealed class SetUserSysTeamValidator : AbstractValidator<SetUserSysTeamRequest>
    {
        public SetUserSysTeamValidator()
        {
            RuleFor(x => x.UserId).GreaterThan(0);
            RuleFor(x => x.TeamId).GreaterThan(0);

        }
    }
}
