using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.EmployeeStatusFeature.ChangeRoomByDate
{
    public sealed record ChangeRoomByDateRequest(int EmployeeId, DateTime StartDate, DateTime EndDate, int RoomId) : IRequest;

}
