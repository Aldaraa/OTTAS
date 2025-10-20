using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.EmployeeStatusFeature.ChangeRoomByDates
{ 
    public sealed class ChangeRoomByDatesValidator : AbstractValidator<ChangeRoomByDatesRequest>
    {
        public ChangeRoomByDatesValidator()
        {
            RuleFor(x => x.EmployeeId).NotEmpty().WithMessage("Employee ID is required.");

            RuleFor(x => x.RoomId).NotEmpty().WithMessage("RoomId us required");

        }
    }


}
