using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.DepartmentGroupConfigFeature.DeleteDepartmentGroupConfig
{
    public sealed class DeleteDepartmentGroupConfigValidator : AbstractValidator<DeleteDepartmentGroupConfigRequest>
    {
        public DeleteDepartmentGroupConfigValidator()
        {
            RuleFor(x => x.Ids).NotEmpty();


        }
    }
}
