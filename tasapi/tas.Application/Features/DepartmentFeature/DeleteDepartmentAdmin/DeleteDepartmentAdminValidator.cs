using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.DepartmentFeature.DeleteDepartmentAdmin
{
    public sealed class DeleteDepartmentAdminValidator : AbstractValidator<DeleteDepartmentAdminRequest>
    {
        public DeleteDepartmentAdminValidator()
        {
            RuleFor(x => x.Id).NotEmpty();
        }
    }
}
