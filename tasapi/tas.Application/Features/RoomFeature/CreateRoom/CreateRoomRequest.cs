using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.RoomFeature.CreateRoom
{
    public sealed record CreateRoomRequest(string Number, int BedCount, int Private, int CampId, int RoomTypeId, int VirtualRoom, int Active) : IRequest;
}
