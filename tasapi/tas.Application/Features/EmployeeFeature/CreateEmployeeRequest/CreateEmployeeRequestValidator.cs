using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.EmployeeFeature.CreateEmployeeRequest
{
    public sealed class RosterExecuteEmployeeValidator : AbstractValidator<CreateEmployeeRequestRequest>
    {
        public RosterExecuteEmployeeValidator()
        {
            RuleFor(x => x.Lastname).NotEmpty().MaximumLength(50);
            RuleFor(x => x.Firstname).NotEmpty().MaximumLength(150);
            RuleFor(x => x.PersonalMobile).MaximumLength(20);
            RuleFor(x => x.Gender).Must(value => value == 0 || value == 1);
            RuleFor(x => x.ADAccount).MaximumLength(50);

        }
    }
}
