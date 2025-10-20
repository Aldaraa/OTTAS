using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.DepartmentGroupConfigFeature.CreateDepartmentGroupConfig
{
    public sealed class CreateDepartmentGroupConfigValidator : AbstractValidator<CreateDepartmentGroupConfigRequest>
    {
        public CreateDepartmentGroupConfigValidator()
        {
            RuleFor(x => x.DepartmentId).NotEmpty();
            RuleFor(x => x.GroupMasterId).NotEmpty();
            RuleFor(x => x.GroupDetailId).NotEmpty();


        }
    }
}
