using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.EmployeeStatusFeature.ChangeRoomByDate
{ 
    public sealed class ChangeRoomByDateValidator : AbstractValidator<ChangeRoomByDateRequest>
    {
        public ChangeRoomByDateValidator()
        {
            RuleFor(x => x.EmployeeId).NotEmpty().WithMessage("Employee ID is required.");

            RuleFor(x => x.StartDate).NotEmpty().WithMessage("StartDate us required");

            RuleFor(x => x.EndDate).NotEmpty().WithMessage("EndDate us required");

            RuleFor(x => x.RoomId).NotEmpty().WithMessage("RoomId us required");

        }
    }


}
