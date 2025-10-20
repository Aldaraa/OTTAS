using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.TransportFeature.ReScheduleMultiple
{
    public sealed class ReScheduleMultipleValidator : AbstractValidator<ReScheduleMultipleRequest>
    {
        public ReScheduleMultipleValidator()
        {
            RuleFor(x => x.ShiftId).NotEmpty().GreaterThan(0).WithMessage("Shift has an invalid value");

            RuleFor(request => request.TransportIds)
           .NotEmpty().WithMessage("TransportIds must not be empty.")
           .Must(ids => ids != null && ids.Any()).WithMessage("TransportIds must contain at least one value.");
        }
    }
}
