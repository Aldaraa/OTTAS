using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.DepartmentFeature.AddDepartmentSupervisor
{
    public sealed class AddDepartmentSupervisorValidator : AbstractValidator<AddDepartmentSupervisorRequest>
    {
        public AddDepartmentSupervisorValidator()
        {
            RuleFor(x => x.DepartmentSupervisorId).NotEmpty();
            RuleFor(x => x.DepartmentId).NotEmpty();

        }
    }
}
