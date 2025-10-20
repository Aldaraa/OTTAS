using MediatR;
using tas.Application.Features.PositionFeature.GetAllRoomType;

namespace tas.Application.Features.RoomTypeFeature.GetAllRoomType
{
    public sealed record GetAllRoomTypeRequest(int? status, int? campId) : IRequest<List<GetAllRoomTypeResponse>>;
}
