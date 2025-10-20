using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.EmployerAdminFeature.DeleteEmployerAdmin
{
    public sealed class DeleteEmployerAdminValidator : AbstractValidator<DeleteEmployerAdminRequest>
    {
        public DeleteEmployerAdminValidator()
        {
            RuleFor(x => x.Id).NotEmpty();
        }
    }
}
