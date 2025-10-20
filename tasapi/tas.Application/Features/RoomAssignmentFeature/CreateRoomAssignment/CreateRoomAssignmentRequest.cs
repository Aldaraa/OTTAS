using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.RoomAssignmentFeature.CreateRoomAssignment;

namespace tas.Application.Features.RoomFeature.CreateRoomAssignment
{
    public sealed record CreateRoomAssignmentRequest(int EmployeeId, int RoomId, DateTime StartDate, DateTime EndDate) : IRequest<CreateRoomAssignmentResponse>;
}
