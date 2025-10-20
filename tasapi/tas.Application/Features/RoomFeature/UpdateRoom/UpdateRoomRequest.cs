using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.RoomFeature.UpdateRoom
{
    public sealed record UpdateRoomRequest(int Id, string Number, int BedCount, int Private, int CampId, int RoomTypeId, int VirtualRoom, int Active) : IRequest;
}
