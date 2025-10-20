using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.DepartmentFeature.AddDepartmentManager
{
    public sealed class AddDepartmentManagerValidator : AbstractValidator<AddDepartmentManagerRequest>
    {
        public AddDepartmentManagerValidator()
        {
            RuleFor(x => x.DepartmentManagerId).NotEmpty();
            RuleFor(x => x.DepartmentId).NotEmpty();

        }
    }
}
