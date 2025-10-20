using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.SysRoleEmployeeReportDepartmentFeature.DeleteSysRoleEmployeeReportDepartment
{
    public sealed class DeleteSysRoleEmployeeReportDepartmentValidator : AbstractValidator<DeleteSysRoleEmployeeReportDepartmentRequest>
    {
        public DeleteSysRoleEmployeeReportDepartmentValidator()
        {
            RuleFor(x => x.Id).NotEmpty();
        }
    }
}
