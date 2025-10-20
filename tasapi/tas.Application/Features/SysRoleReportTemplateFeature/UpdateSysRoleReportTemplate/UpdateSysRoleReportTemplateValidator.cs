using FluentValidation;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.StateFeature.CreateState;

namespace tas.Application.Features.SysRoleReportTemplateFeature.UpdateSysRoleReportTemplate
{ 
    public sealed class UpdateSysRoleReportTemplateValidator : AbstractValidator<UpdateSysRoleReportTemplateRequest>
    {
        public UpdateSysRoleReportTemplateValidator()
        {
            RuleFor(x => x.RoleId).GreaterThan(0);

        }
    }
}
