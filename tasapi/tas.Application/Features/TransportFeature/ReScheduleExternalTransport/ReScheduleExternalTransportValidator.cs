using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.TransportFeature.ReScheduleExternalTransport
{
    public sealed class ReScheduleExternalTransportValidator : AbstractValidator<ReScheduleExternalTransportRequest>
    {
        public ReScheduleExternalTransportValidator()
        {
           // RuleFor(x => x.ShiftId).NotEmpty().GreaterThan(0).WithMessage("RoomStatus has an invalid value");

           // RuleFor(request => request.TransportIds)
           //.NotEmpty().WithMessage("TransportIds must not be empty.")
           //.Must(ids => ids != null && ids.Any()).WithMessage("TransportIds must contain at least one value.");
        }
    }
}
