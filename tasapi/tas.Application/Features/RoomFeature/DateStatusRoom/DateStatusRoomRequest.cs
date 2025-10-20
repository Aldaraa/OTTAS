using MediatR;
using tas.Application.Features.RoomFeature.DateStatusRoom;

namespace tas.Application.Features.RoomFeature.DateStatusRoom
{
    public sealed record DateStatusRoomRequest(
            DateTime CurrentDate, 
            int RoomId
        ) : IRequest<DateStatusRoomResponse>;
}
