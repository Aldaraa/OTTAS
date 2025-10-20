using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.EmployeeStatusFeature.CalendarBookingRoomAssign
    { 
    public sealed class CalendarBookingRoomAssignValidator : AbstractValidator<CalendarBookingRoomAssignRequest>
    {
        public CalendarBookingRoomAssignValidator()
        {
            RuleFor(x => x.EmployeeId).NotEmpty().WithMessage("Employee ID is required.");

            RuleFor(x => x.RoomDateStatus).NotEmpty().WithMessage("Room date status list is required.")
                .ForEach(roomDateDataValidator => roomDateDataValidator.SetValidator(new RoomDateDataValidator()));
        }
    }

    public sealed class RoomDateDataValidator : AbstractValidator<RoomDateData>
    {
        public RoomDateDataValidator()
        {
            RuleFor(x => x.EventDate).NotEmpty().WithMessage("Event date is required.");

            RuleFor(x => x.RoomId).NotEmpty().WithMessage("Room ID is required.");
        }
    }
}
