using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.SysTeamFeature.SetMenuSysTeam;

namespace tas.Application.Features.SysRoleFeature.AddEmployee
{

    public sealed class AddEmployeeValidator : AbstractValidator<AddEmployeeRequest>
    {
        public AddEmployeeValidator()
        {
            RuleFor(x => x.EmployeeId).NotEmpty().GreaterThan(0);
            RuleFor(x => x.RoleId).NotEmpty().GreaterThan(0);
            RuleFor(x => x.ReadOnlyAccess).Must(value => value == 0 || value == 1);
        }

    }
}
