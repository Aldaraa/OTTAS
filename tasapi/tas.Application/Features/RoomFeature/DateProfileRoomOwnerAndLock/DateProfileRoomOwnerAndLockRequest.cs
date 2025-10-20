using MediatR;
using tas.Application.Features.RoomOwnerAndLockFeature.DateProfileRoomOwnerAndLock;
using tas.Domain.Common;

namespace tas.Application.Features.RoomOwnerAndLockFeature.DateProfileRoomOwnerAndLock
{
    public sealed record DateProfileRoomOwnerAndLockRequest(

            DateTime CurrentDate,
            int RoomId
        ) : IRequest<DateProfileRoomOwnerAndLockResponse>;



}
