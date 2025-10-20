using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.MultipleBookingFeature.MultipleBookingPreviewTransport
{
    public sealed class MultipleBookingPreviewTransportValidator : AbstractValidator<MultipleBookingPreviewTransportRequest>
    {
        public MultipleBookingPreviewTransportValidator()
        {
            // Validate EmployeeIds list is not null or empty
            RuleFor(request => request.EmployeeIds)
                .NotNull()
                .WithMessage("Employee IDs cannot be null.")
                .NotEmpty()
                .WithMessage("Employee IDs cannot be empty.");

            // Validate firstScheduleId is greater than 0
            RuleFor(request => request.firsScheduleId)
                .GreaterThan(0)
                .WithMessage("First Schedule ID must be greater than 0.");

            // Validate lastScheduleId if present
            RuleFor(request => request.lastSheduleId)
                .GreaterThan(0)
                .When(request => request.lastSheduleId.HasValue)
                .WithMessage("Last Schedule ID, if provided, must be greater than 0.");
        }
    }
}
