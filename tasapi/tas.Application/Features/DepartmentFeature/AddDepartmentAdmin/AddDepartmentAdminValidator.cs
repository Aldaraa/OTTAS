using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.DepartmentFeature.AddDepartmentAdmin
{
    public sealed class AddDepartmentAdminValidator : AbstractValidator<AddDepartmentAdminRequest>
    {
        public AddDepartmentAdminValidator()
        {
            RuleFor(x => x.DepartmentAdminId).NotEmpty();
            RuleFor(x => x.DepartmentId).NotEmpty();

        }
    }
}
