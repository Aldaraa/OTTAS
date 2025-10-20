using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.DepartmentFeature.UpdateDepartment;

namespace tas.Application.Features.DepartmenteFeature.UpdateDepartmente
{
    public sealed class UpdateDepartmentValidator : AbstractValidator<UpdateDepartmentRequest>
    {
        public UpdateDepartmentValidator()
        {
            RuleFor(x => x.Id).NotEmpty();
            RuleFor(x => x.Name).NotEmpty().MaximumLength(150);
            RuleFor(x => x.ParentDepartmentId).NotEmpty();
        }
    }
}
