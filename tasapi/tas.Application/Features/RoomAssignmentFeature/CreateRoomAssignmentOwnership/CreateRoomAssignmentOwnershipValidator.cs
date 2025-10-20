using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.RoomFeature.CreateRoomAssignmentOwnership
{
    public sealed class CreateRoomAssignmentOwnershipValidator : AbstractValidator<CreateRoomAssignmentOwnershipRequest>
    {
        public CreateRoomAssignmentOwnershipValidator()
        {
            RuleFor(x => x.EmployeeId).NotEmpty();
            RuleFor(x => x.RoomId).NotEmpty();

        }
    }
}
