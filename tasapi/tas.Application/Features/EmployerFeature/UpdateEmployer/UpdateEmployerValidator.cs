using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.EmployerFeature.UpdateEmployer;

namespace tas.Application.Features.EmployereFeature.UpdateEmployere
{
    public sealed class UpdateEmployerValidator : AbstractValidator<UpdateEmployerRequest>
    {
        public UpdateEmployerValidator()
        {
            RuleFor(x => x.Id).NotEmpty();
            RuleFor(x => x.Code).NotEmpty().MaximumLength(50);
            RuleFor(x => x.Description).NotEmpty();
        }
    }
}
