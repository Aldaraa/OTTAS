using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.EmployeeFeature.UpdateEmployee;

namespace tas.Application.Features.EmployeeStatusFeature.VisualStatusDateChangeBulk
{
    public sealed class VisualStatusDateChangeBulkValidator : AbstractValidator<VisualStatusDateChangeBulkRequest>
    {
        public VisualStatusDateChangeBulkValidator()
        {
            RuleFor(request => request.EmployeeIds)
             .NotEmpty().WithMessage("Employee IDs cannot be empty.")
             .Must(ids => ids.All(id => id > 0)).WithMessage("All Employee IDs must be greater than 0.");

            RuleFor(request => request.ShiftId)
                .GreaterThan(0).WithMessage("Shift ID must be greater than 0.");

            RuleFor(request => request.startDate)
                .LessThanOrEqualTo(request => request.endDate).WithMessage("Start date must be before or equal to end date.");

            RuleFor(request => request.endDate)
                .GreaterThanOrEqualTo(request => request.startDate).WithMessage("End date must be after or equal to start date.");
        }
    }
}
