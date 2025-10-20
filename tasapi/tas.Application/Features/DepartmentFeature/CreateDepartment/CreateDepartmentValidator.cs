using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.DepartmentFeature.CreateDepartment
{
    public sealed class CreateDepartmentValidator : AbstractValidator<CreateDepartmentRequest>
    {
        public CreateDepartmentValidator()
        {
            RuleFor(x => x.Name).NotEmpty().MaximumLength(150);
            RuleFor(x => x.ParentDepartmentId).NotEmpty();

        }
    }
}
