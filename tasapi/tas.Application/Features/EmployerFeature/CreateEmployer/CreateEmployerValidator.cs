using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.EmployerFeature.CreateEmployer
{
    public sealed class CreateEmployerValidator : AbstractValidator<CreateEmployerRequest>
    {
        public CreateEmployerValidator()
        {
            RuleFor(x => x.Code).NotEmpty().MaximumLength(50);
            RuleFor(x => x.Description).NotEmpty();

        }
    }
}
