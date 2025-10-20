using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.DepartmentFeature.DeleteDepartmentSupervisor
{
    public sealed class DeleteDepartmentSupervisorValidator : AbstractValidator<DeleteDepartmentSupervisorRequest>
    {
        public DeleteDepartmentSupervisorValidator()
        {
            RuleFor(x => x.Id).NotEmpty();
        }
    }
}
