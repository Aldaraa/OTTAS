using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.SysRoleEmployeeReportEmployerFeature.DeleteSysRoleEmployeeReportEmployer
{
    public sealed class DeleteSysRoleEmployeeReportEmployerValidator : AbstractValidator<DeleteSysRoleEmployeeReportEmployerRequest>
    {
        public DeleteSysRoleEmployeeReportEmployerValidator()
        {
            RuleFor(x => x.Id).NotEmpty();
        }
    }
}
