using MediatR;
using tas.Application.Features.RoomFeature.GetVirtualRoom;

namespace tas.Application.Features.RoomFeature.GetVirtualRoom
{
    public sealed record GetVirtualRoomRequest : IRequest<GetVirtualRoomResponse>;
}
