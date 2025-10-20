using MediatR;
using tas.Application.Features.RoomFeature.GetAllRoom;

namespace tas.Application.Features.RoomFeature.GetAllRoom
{
    public sealed record GetAllRoomRequest(int? status) : IRequest<List<GetAllRoomResponse>>;
}
