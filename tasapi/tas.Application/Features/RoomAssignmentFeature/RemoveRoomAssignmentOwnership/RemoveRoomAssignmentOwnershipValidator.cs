using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.RoomFeature.RemoveRoomAssignmentOwnership
{
    public sealed class RemoveRoomAssignmentOwnershipValidator : AbstractValidator<RemoveRoomAssignmentOwnershipRequest>
    {
        public RemoveRoomAssignmentOwnershipValidator()
        {
            RuleFor(x => x.EmployeeId).NotEmpty();
            RuleFor(x => x.StartDate).NotEmpty();

        }
    }
}
