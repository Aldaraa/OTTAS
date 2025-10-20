using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.RoomFeature.CreateRoomAssignment
{
    public sealed class CreateRoomAssignmentValidator : AbstractValidator<CreateRoomAssignmentRequest>
    {
        public CreateRoomAssignmentValidator()
        {
            RuleFor(x => x.EmployeeId).NotEmpty();
            RuleFor(x => x.RoomId).NotEmpty();
            RuleFor(x => x.StartDate).NotEmpty();
            RuleFor(x => x.EndDate).NotEmpty();

        }
    }
}
