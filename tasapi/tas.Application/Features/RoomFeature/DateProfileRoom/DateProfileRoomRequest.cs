using MediatR;
using tas.Application.Features.RoomFeature.DateProfileRoom;
using tas.Domain.Common;

namespace tas.Application.Features.RoomFeature.DateProfileRoom
{
    public sealed record DateProfileRoomRequest(
DateProfileRoomSearchRequest model
        ) : BasePagenationRequest, IRequest<DateProfileRoomResponse>;


    public record DateProfileRoomSearchRequest(

            DateTime CurrentDate,
            int RoomId
    );


}
