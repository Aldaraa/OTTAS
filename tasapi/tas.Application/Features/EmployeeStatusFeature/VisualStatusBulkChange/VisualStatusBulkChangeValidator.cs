using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.EmployeeFeature.UpdateEmployee;

namespace tas.Application.Features.EmployeeStatusFeature.VisualStatusBulkChange
{
    public sealed class VisualStatusBulkChangeValidator : AbstractValidator<VisualStatusBulkChangeRequest>
    {
        public VisualStatusBulkChangeValidator()
        {
            RuleFor(request => request.EmployeeId)
              .NotEmpty().WithMessage("EmployeeId is required.");

            RuleFor(request => request.StartDate)
                .NotEmpty().WithMessage("StartDate is required.")
                .Must(BeValidDate).WithMessage("StartDate must be a valid date.");

            RuleFor(request => request.EndDate)
                .NotEmpty().WithMessage("EndDate is required.")
                .Must(BeValidDate).WithMessage("EndDate must be a valid date.")
                .GreaterThan(request => request.StartDate).WithMessage("EndDate must be greater than StartDate.");

            RuleFor(request => request.ShiftId)
                .NotEmpty().WithMessage("ShiftId is required.")
                .GreaterThan(0).WithMessage("ShiftId must be greater than zero.");
        }

        private bool BeValidDate(DateTime date)
        {
            return date != DateTime.MinValue && date != DateTime.MaxValue;
        }
    }

}
