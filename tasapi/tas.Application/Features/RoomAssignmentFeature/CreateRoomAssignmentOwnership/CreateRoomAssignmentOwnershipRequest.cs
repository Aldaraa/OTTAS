using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.RoomAssignmentFeature.CreateRoomAssignmentOwnership;

namespace tas.Application.Features.RoomFeature.CreateRoomAssignmentOwnership
{
    public sealed record CreateRoomAssignmentOwnershipRequest(DateTime startDate, int EmployeeId, int RoomId) : IRequest<List<CreateRoomAssignmentOwnershipResponse>>;
}
