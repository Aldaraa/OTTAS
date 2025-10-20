using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.EmployeeFeature.UpdateEmployee;

namespace tas.Application.Features.EmployeeFeature.UpdateEmployere
{
    public sealed class UpdateEmployeeValidator : AbstractValidator<UpdateEmployeeRequest>
    {
        public UpdateEmployeeValidator()
        {
            RuleFor(x => x.Id).GreaterThan(0);
            RuleFor(x => x.Lastname).NotEmpty().MaximumLength(50);
            RuleFor(x => x.Firstname).NotEmpty().MaximumLength(50);
            RuleFor(x => x.Gender).Must(value => value == 0 || value == 1);
            RuleFor(x => x.ADAccount).MaximumLength(50);
        }
    }
}
