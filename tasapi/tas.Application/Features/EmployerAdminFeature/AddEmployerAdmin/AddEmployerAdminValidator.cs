using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.EmployerAdminFeature.AddEmployerAdmin
{
    public sealed class AddEmployerAdminValidator : AbstractValidator<AddEmployerAdminRequest>
    {
        public AddEmployerAdminValidator()
        {
            RuleFor(x => x.EmployerId).NotEmpty();
            RuleFor(x => x.EmployeeId).NotEmpty();

        }
    }
}
