using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.RoomFeature.RemoveRoomAssignmentOwnership
{
    public sealed record RemoveRoomAssignmentOwnershipRequest(DateTime StartDate, int EmployeeId) : IRequest;
}
