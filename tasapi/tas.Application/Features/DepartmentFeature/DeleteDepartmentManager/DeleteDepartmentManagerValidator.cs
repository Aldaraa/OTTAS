using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.DepartmentFeature.DeleteDepartmentManager
{
    public sealed class DeleteDepartmentManagerValidator : AbstractValidator<DeleteDepartmentManagerRequest>
    {
        public DeleteDepartmentManagerValidator()
        {
            RuleFor(x => x.Id).NotEmpty();
        }
    }
}
