using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.SysRoleEmployeeReportDepartmentFeature.AddSysRoleEmployeeReportDepartment
{
    public sealed class AddSysRoleEmployeeReportDepartmentValidator : AbstractValidator<AddSysRoleEmployeeReportDepartmentRequest>
    {
        public AddSysRoleEmployeeReportDepartmentValidator()
        {
            RuleFor(x => x.DepartmentId).NotEmpty();
            RuleFor(x => x.EmployeeId).NotEmpty();

        }
    }
}
