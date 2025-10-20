using MediatR;
using tas.Application.Features.PositionFeature.GetAllPosition;
using tas.Application.Features.RoomFeature.MonthStatusRoom;
using tas.Domain.Common;

namespace tas.Application.Features.EmployeeStatusFeature.RoomBookingByRoom
{
    //public sealed record RoomBookingByRoomRequest(int RoomId, DateTime StartDate, DateTime EndDate) : IRequest<List<RoomBookingByRoomResponse>>;



    public sealed record RoomBookingByRoomRequest(RoomBookingByRoomRequestData model) : BasePagenationRequest, IRequest<RoomBookingByRoomResponse>;


    public record RoomBookingByRoomRequestData(

         int RoomId, DateTime StartDate, DateTime EndDate
    );





}
