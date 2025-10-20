using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.SysRoleEmployeeReportEmployerFeature.AddSysRoleEmployeeReportEmployer
{
    public sealed class AddSysRoleEmployeeReportEmployerValidator : AbstractValidator<AddSysRoleEmployeeReportEmployerRequest>
    {
        public AddSysRoleEmployeeReportEmployerValidator()
        {
            RuleFor(x => x.EmployerId).NotEmpty();
            RuleFor(x => x.EmployeeId).NotEmpty();

        }
    }
}
