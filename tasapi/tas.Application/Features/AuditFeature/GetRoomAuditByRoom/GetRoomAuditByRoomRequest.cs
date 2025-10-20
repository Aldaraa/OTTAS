using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.AuditFeature.GetRoomAuditByRoom
{
    public sealed record GetRoomAuditByRoomRequest(int RoomId, DateTime startDate, DateTime endDate) : IRequest<GetRoomAuditByRoomResponse>;
}
