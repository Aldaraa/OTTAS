using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.EmployeeFeature.ChangeEmployeeLocation
{
    public sealed class ChangeEmployeeLocationValidator : AbstractValidator<ChangeEmployeeLocationRequest>
    {
        public ChangeEmployeeLocationValidator()
        {
            RuleForEach(x => x.data)
                      .ChildRules(EmployeeLocation =>
                      {
                          EmployeeLocation.RuleFor(data => data.employeeId).NotEmpty().WithMessage("Employee is required.");
                          EmployeeLocation.RuleFor(data => data.LocationId).NotEmpty().WithMessage("Location is required");

                      });
        }
    }
}
