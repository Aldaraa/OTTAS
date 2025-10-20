using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.EmployeeStatusFeature.ChangeRoomByDates
{
    public sealed record ChangeRoomByDatesRequest(int EmployeeId, List<DateTime> Dates, int RoomId) : IRequest<ChangeRoomByDatesResponse>;

}
