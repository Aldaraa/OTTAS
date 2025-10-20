using MediatR;
using tas.Application.Features.RoomFeature.DateProfileRoomDetail;

namespace tas.Application.Features.RoomFeature.DateProfileRoomDetail
{
    public sealed record DateProfileRoomDetailRequest(
            DateTime CurrentDate, 
            int RoomId
        ) : IRequest<List<DateProfileRoomDetailResponse>>;
}
