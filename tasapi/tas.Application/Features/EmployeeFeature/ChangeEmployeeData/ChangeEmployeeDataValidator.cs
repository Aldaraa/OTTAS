using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.EmployeeFeature.ChangeEmployeeData
{
    public sealed class CreateCostCodeValidator : AbstractValidator<ChangeEmployeeDataRequest>
    {
        public CreateCostCodeValidator()
        {
            RuleForEach(x => x.data)
                      .ChildRules(employeeData =>
                      {
                          employeeData.RuleFor(data => data.employeeId).NotEmpty().WithMessage("EmployeeId is required.");
                          employeeData.RuleFor(data => data.DataId).NotEmpty().WithMessage("Id is required.");
                          employeeData.RuleFor(data => data.startDate).Must(BeValidDate).WithMessage("Start date is required and must be a valid date.");
                          employeeData.RuleFor(data => data.endDate)
                              .Must((data, endDate) => endDate == null || endDate > data.startDate)
                              .WithMessage("End date must be null or greater than the start date.");
                      });
        }

        private bool BeValidDate(DateTime date)
        {
            return date != default(DateTime);
        }
    }
}
