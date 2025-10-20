using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.EmployeeProfileStatusFeature.GetDateRangeProfileStatus;
using tas.Application.Features.EmployeeStatusFeature.CalendarBookingEmployee;
using tas.Application.Features.EmployeeStatusFeature.CalendarBookingRoomAssign;
using tas.Application.Features.EmployeeStatusFeature.ChangeRoomByDate;
using tas.Application.Features.EmployeeStatusFeature.ChangeRoomByDates;
using tas.Application.Features.EmployeeStatusFeature.DateLastEmployeeStatus;
using tas.Application.Features.EmployeeStatusFeature.GetDateRangeStatus;
using tas.Application.Features.EmployeeStatusFeature.RoomBookingByRoom;
using tas.Application.Features.EmployeeStatusFeature.RoomBookingEmployee;
using tas.Application.Features.EmployeeStatusFeature.VisualStatusBulkChange;
using tas.Application.Features.EmployeeStatusFeature.VisualStatusDateChange;
using tas.Application.Features.EmployeeStatusFeature.VisualStatusDateChangeBulk;
using tas.Application.Features.EmployeeStatusFeature.VisualStatusGetEmployee;
using tas.Domain.Entities;

namespace tas.Application.Repositories
{
    public interface IEmployeeStatusRepository : IBaseRepository<EmployeeStatus>
    {
        Task<List<RoomBookingEmployeeResponse>> EmployeeRoombooking(RoomBookingEmployeeRequest request, CancellationToken cancellationToken);

        Task<RoomBookingByRoomResponse> RoombookingByRoom(RoomBookingByRoomRequest request, CancellationToken cancellationToken);


        Task<List<VisualStatusGetEmployeeResponse>> VisualStatusGetEmployee(VisualStatusGetEmployeeRequest request, CancellationToken cancellationToken);

        Task<List<CalendarBookingEmployeeResponse>> EmployeeBookingViewCalendar(CalendarBookingEmployeeRequest request, CancellationToken cancellationToken);


        Task CalendarRoomAssign(CalendarBookingRoomAssignRequest request, CancellationToken cancellationToken);

        Task VisualStatusDateChange(VisualStatusDateChangeRequest request, CancellationToken cancellationToken);

        Task<List<VisualStatusDateChangeBulkResponse>> VisualStatusDateChangeBulk(VisualStatusDateChangeBulkRequest request, CancellationToken cancellationToken);

        Task VisualStatusBulkChange(VisualStatusBulkChangeRequest request, CancellationToken cancellationToken);

        Task ChangeRoomByDateRangeAssign(ChangeRoomByDateRequest request, CancellationToken cancellationToken);

        Task<ChangeRoomByDatesResponse> ChangeRoomByDatesAssign(ChangeRoomByDatesRequest request, CancellationToken cancellationToken);



        Task<List<GetDateRangeStatusResponse>> GetDateRangeStatus(GetDateRangeStatusRequest request, CancellationToken cancellationToken);

        Task<List<GetDateRangeProfileStatusResponse>> GetDateRangeProfileStatus(GetDateRangeProfileStatusRequest request, CancellationToken cancellationToken);

        Task<DateLastEmployeeStatusResponse> DateLastEmployeeStatus(DateLastEmployeeStatusRequest request, CancellationToken cancellationToken);




    }
}
