using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.EmployeeFeature.DeActiveEmployee;

namespace tas.Application.Features.EmployeeFeature.ReActiveEmployee
{
    public class ReActiveEmployeeValidator : AbstractValidator<ReActiveEmployee>
    {
        public ReActiveEmployeeValidator()
        {
            RuleFor(e => e.EmployeeId).NotEmpty().GreaterThan(0);
            RuleFor(e => e.EventDate).NotEmpty();
        }
    }

    public class ReActiveEmployeeRequestValidator : AbstractValidator<ReActiveEmployeeRequest>
    {
        public ReActiveEmployeeRequestValidator()
        {
            RuleForEach(r => r.Employees).SetValidator(new ReActiveEmployeeValidator());
        }
    }
}
