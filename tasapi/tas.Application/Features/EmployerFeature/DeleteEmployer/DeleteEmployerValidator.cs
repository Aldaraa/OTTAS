using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.EmployerFeature.DeleteEmployer
{
    public sealed class DeleteEmployerValidator : AbstractValidator<DeleteEmployerRequest>
    {
        public DeleteEmployerValidator()
        {
            RuleFor(x => x.Id).NotEmpty();
        }
    }
}
