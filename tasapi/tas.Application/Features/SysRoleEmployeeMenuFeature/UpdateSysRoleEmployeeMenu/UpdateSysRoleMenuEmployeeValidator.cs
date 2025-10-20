using FluentValidation;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.StateFeature.CreateState;

namespace tas.Application.Features.SysRoleEmployeeMenuFeature.UpdateSysRoleEmployeeMenu
{ 
    public sealed class UpdateSysRoleEmployeeMenuValidator : AbstractValidator<UpdateSysRoleEmployeeMenuRequest>
    {
        public UpdateSysRoleEmployeeMenuValidator()
        {
            RuleFor(x => x.EmployeeId).GreaterThan(0);

        }
    }
}
