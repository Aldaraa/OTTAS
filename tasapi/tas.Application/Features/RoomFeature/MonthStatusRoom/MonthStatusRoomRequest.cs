using MediatR;
using tas.Application.Features.RoomFeature.DateProfileRoom;
using tas.Application.Features.RoomFeature.MonthStatusRoom;
using tas.Domain.Common;

namespace tas.Application.Features.RoomFeature.MonthStatusRoom
{
    public sealed record MonthStatusRoomRequest(MonthStatusRoomRequestData model) : BasePagenationRequest, IRequest<MonthStatusRoomResponse>;


    public record MonthStatusRoomRequestData(

            DateTime CurrentDate,
            int RoomId,
            string? keyword
    );




}
