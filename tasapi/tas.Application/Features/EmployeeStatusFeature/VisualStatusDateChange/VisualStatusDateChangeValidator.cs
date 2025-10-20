using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.EmployeeFeature.UpdateEmployee;

namespace tas.Application.Features.EmployeeStatusFeature.VisualStatusDateChange
{
    public sealed class VisualStatusDateChangeValidator : AbstractValidator<VisualStatusDateChangeRequest>
    {
        public VisualStatusDateChangeValidator()
        {
            RuleFor(request => request.EmployeeId)
                .NotEmpty().WithMessage("EmployeeId is required.");

            RuleFor(request => request.StatusDates)
                .NotEmpty().WithMessage("StatusDates list is required.")
                .ForEach(rule => rule.SetValidator(new StatusDatesValidator()));
        }
    }

    public class StatusDatesValidator : AbstractValidator<StatusDate>
    {
        public StatusDatesValidator()
        {
            RuleFor(statusDate => statusDate.EventDate)
                .NotEmpty().WithMessage("EventDate is required.");

            RuleFor(statusDate => statusDate.ShiftId)
                .NotEmpty().WithMessage("ShiftId is required.")
                .GreaterThan(0).WithMessage("ShiftId must be greater than zero.");
        }
    }
}
