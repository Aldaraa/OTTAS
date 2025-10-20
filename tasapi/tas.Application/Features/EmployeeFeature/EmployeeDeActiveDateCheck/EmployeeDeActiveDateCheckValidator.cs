using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.EmployeeFeature.EmployeeDeActiveDateCheck
{

    public class EmployeeDeActiveDateCheckRequestValidator : AbstractValidator<EmployeeDeActiveDateCheckRequest>
    {
        public EmployeeDeActiveDateCheckRequestValidator()
        {
            RuleFor(r => r.EventDate).NotEmpty();
            RuleFor(r => r.EmployeeId).NotNull().GreaterThan(0);

        }
    }
}
