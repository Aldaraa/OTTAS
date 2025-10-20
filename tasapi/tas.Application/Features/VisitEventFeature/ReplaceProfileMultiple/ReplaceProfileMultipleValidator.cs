using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.VisitEventFeature.ReplaceProfileMultiple
{
    public sealed class ReplaceProfileMultipleValidator : AbstractValidator<ReplaceProfileMultipleRequest>
    {
        public ReplaceProfileMultipleValidator()
        {
            RuleFor(x => x.Employees).NotEmpty().WithMessage("Employees list must not be empty");
            RuleForEach(x => x.Employees).SetValidator(new ReplaceProfileMultipleRequestEmployeesValidator());
            RuleFor(x => x.EventId).GreaterThan(0).WithMessage("EventId must be greater than 0");
        }

        public class ReplaceProfileMultipleRequestEmployeesValidator : AbstractValidator<ReplaceProfileMultipleRequestEmployees>
        {
            public ReplaceProfileMultipleRequestEmployeesValidator()
            {
                RuleFor(x => x.oldEmployeeId).GreaterThan(0).WithMessage("oldEmployeeId must be greater than 0");
                RuleFor(x => x.newEmployeeId).GreaterThan(0).WithMessage("newEmployeeId must be greater than 0");
            }
        }
    }
}
