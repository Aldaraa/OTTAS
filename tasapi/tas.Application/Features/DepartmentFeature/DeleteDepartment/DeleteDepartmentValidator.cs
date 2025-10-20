using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.DepartmentFeature.UpdateDepartment;

namespace tas.Application.Features.DepartmentFeature.DeleteDepartment
{
    public sealed class DeleteDepartmentValidator : AbstractValidator<DeleteDepartmentRequest>
    {
        public DeleteDepartmentValidator()
        {
            RuleFor(x => x.Id).NotEmpty();
        }
    }
}
