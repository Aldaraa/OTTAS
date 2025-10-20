using MediatR;
using tas.Application.Features.RoomFeature.GetRoom;

namespace tas.Application.Features.RoomFeature.GetRoom
{
    public sealed record GetRoomRequest(int Id) : IRequest<GetRoomResponse>;
}
