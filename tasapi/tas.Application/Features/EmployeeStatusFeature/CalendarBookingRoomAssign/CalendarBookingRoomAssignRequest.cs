using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.EmployeeStatusFeature.CalendarBookingRoomAssign
{
    public sealed record CalendarBookingRoomAssignRequest(int EmployeeId, List<RoomDateData> RoomDateStatus) : IRequest;

    public sealed record RoomDateData
    { 
        public DateTime EventDate { get; set; }

        public int RoomId { get; set; }
    }
}
