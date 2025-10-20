using FluentValidation;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.StateFeature.CreateState;

namespace tas.Application.Features.SysRoleEmployeeReportTemplateFeature.UpdateSysRoleEmployeeReportTemplate
{ 
    public sealed class UpdateSysRoleEmployeeReportTemplateValidator : AbstractValidator<UpdateSysRoleEmployeeReportTemplateRequest>
    {
        public UpdateSysRoleEmployeeReportTemplateValidator()
        {
            RuleFor(x => x.EmployeeId).GreaterThan(0);

        }
    }
}
