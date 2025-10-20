using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.SysTeamFeature.SetMenuSysTeam;

namespace tas.Application.Features.SysRoleFeature.UpdateReadOnlyAccesss
{

    public sealed class UpdateReadOnlyAccesssValidator : AbstractValidator<UpdateReadOnlyAccesssRequest>
    {

        public UpdateReadOnlyAccesssValidator()
        {
            RuleFor(x => x.Id).NotEmpty().GreaterThan(0);
            RuleFor(x => x.ReadOnlyAccess).Must(value => value == 0 || value == 1);
        }
    }
}
