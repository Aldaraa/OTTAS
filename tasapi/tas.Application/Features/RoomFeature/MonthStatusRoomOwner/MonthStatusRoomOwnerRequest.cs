using MediatR;
using tas.Application.Features.RoomFeature.MonthStatusRoomOwner;
using tas.Domain.Common;

namespace tas.Application.Features.RoomFeature.MonthStatusRoomOwner
{
    public sealed record MonthStatusRoomOwnerRequest(
            DateTime CurrentDate, 
            int RoomId
        ) : IRequest<List<MonthStatusRoomOwnerResponse>>;
}
