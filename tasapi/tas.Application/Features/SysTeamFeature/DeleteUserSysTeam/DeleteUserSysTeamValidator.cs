using FluentValidation;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.StateFeature.CreateState;

namespace tas.Application.Features.SysTeamFeature.DeleteUserSysTeam
{
    public sealed class DeleteUserSysTeamValidator : AbstractValidator<DeleteUserSysTeamRequest>
    {
        public DeleteUserSysTeamValidator()
        {
            RuleFor(x => x.UserId).GreaterThan(0);
            RuleFor(x => x.TeamId).GreaterThan(0);

        }
    }
}
