using FluentValidation;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.StateFeature.CreateState;

namespace tas.Application.Features.SysRoleEmployeeDashboardFeature.UpdateSysRoleEmployeeDashboard
{ 
    public sealed class UpdateSysRoleEmployeeDashboardValidator : AbstractValidator<UpdateSysRoleEmployeeDashboardRequest>
    {
        public UpdateSysRoleEmployeeDashboardValidator()
        {
            RuleFor(x => x.EmployeeId).GreaterThan(0);

        }
    }
}
