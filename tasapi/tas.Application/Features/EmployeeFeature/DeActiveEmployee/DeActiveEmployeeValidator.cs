using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.EmployeeFeature.DeActiveEmployee
{
    public class DeActiveEmployeeValidator : AbstractValidator<DeActiveEmployee>
    {
        public DeActiveEmployeeValidator()
        {
            RuleFor(e => e.EmployeeId).NotEmpty().GreaterThan(0);
            RuleFor(e => e.EventDate).NotEmpty();
            RuleFor(e => e.Comment).MaximumLength(500);
            RuleFor(e => e.DemobTypeTypeId).GreaterThan(0).When(e => e.DemobTypeTypeId != null);
        }
    }

    public class DeActiveEmployeeRequestValidator : AbstractValidator<DeActiveEmployeeRequest>
    {
        public DeActiveEmployeeRequestValidator()
        {
            RuleForEach(r => r.Employees).SetValidator(new DeActiveEmployeeValidator());
        }
    }
}
