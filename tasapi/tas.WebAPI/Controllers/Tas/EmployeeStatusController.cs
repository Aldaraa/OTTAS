using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using tas.Application.Features.AuthenticationFeature.LoginUser;
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
using tas.Application.Service;
using tas.Domain.Entities;

namespace tas.WebAPI.Controllers.Tas
{

    [Route("api/tas/[controller]")]
    [ApiController]
    [RoleAuthorize]
    public class EmployeeStatusController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<EmployeeStatusController> _logger;
        private readonly NotificationHub _notificationHub;
        private readonly IMemoryCache _memoryCache;

        public EmployeeStatusController(IMediator mediator, ILogger<EmployeeStatusController> logger, NotificationHub notificationHub, IMemoryCache memoryCache)
        {
            _mediator = mediator;
            _logger = logger;
            _notificationHub = notificationHub;
            _memoryCache = memoryCache;
        }

        [HttpGet("employeebooking")]
        public async Task<ActionResult<List<RoomBookingEmployeeResponse>>> EmployeeRoombooking(int employeeId, DateTime startDate, DateTime endDate, CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(new RoomBookingEmployeeRequest(employeeId, startDate, endDate), cancellationToken);
            return Ok(response);
        }

        [HttpPost("roombookingbyroom")]
        public async Task<ActionResult<RoomBookingEmployeeResponse>> RoomBookingByRoom(RoomBookingByRoomRequest request, CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(request, cancellationToken);
            return Ok(response);
        }


        [HttpGet("visualstatusdates/{EmployeeId}/{startDate}")]
        public async Task<ActionResult<List<VisualStatusGetEmployeeResponse>>> GetVisualStatusDate(int EmployeeId, DateTime startDate, CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(new VisualStatusGetEmployeeRequest(EmployeeId, startDate), cancellationToken);
            return Ok(response);
        }

        [HttpGet("roombookingviewcalendar/{EmployeeId}/{startDate}")]
        public async Task<ActionResult<List<VisualStatusGetEmployeeResponse>>> RoomBookingViewCalendar(int EmployeeId, DateTime startDate, CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(new CalendarBookingEmployeeRequest(EmployeeId, startDate), cancellationToken);
            return Ok(response);
        }


        [HttpGet("profilebydate/{EmployeeId}/{startDate}/{endDate}")]
        public async Task<ActionResult<List<GetDateRangeProfileStatusResponse>>> ProfileByDdateStatus(int EmployeeId, DateTime startDate, DateTime endDate, CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(new GetDateRangeProfileStatusRequest(EmployeeId, startDate, endDate), cancellationToken);
            return Ok(response);
        }

        [HttpGet("annualyear/{EmployeeId}/{startDate}/{monthDuration}")]
        public async Task<ActionResult<List<VisualStatusGetEmployeeResponse>>> GetAnnualYear(int EmployeeId, DateTime startDate, int monthDuration, CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(new GetDateRangeStatusRequest(EmployeeId, startDate, monthDuration), cancellationToken);
            return Ok(response);
        }


        [HttpPost("calendarroombookingassign")]
        public async Task<ActionResult<List<VisualStatusGetEmployeeResponse>>> CalendarRoombookingAssign(CalendarBookingRoomAssignRequest request, CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(request, cancellationToken);
            await _notificationHub.BroadCastAllUsersUpdateNotification();
            return Ok(response);
        }

        [HttpPost("VisualStatusDateChange")]
        public async Task<ActionResult> VisualStatusDateChange(VisualStatusDateChangeRequest request, CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(request, cancellationToken);
            string cacheEntityName = $"Employee_{request.EmployeeId}";
            _memoryCache.Remove($"API::{cacheEntityName}");



            await _notificationHub.BroadCastAllUsersUpdateNotification();
            return Ok(response);
        }

        [HttpPost("VisualStatusBulkChange")]
        public async Task<ActionResult> VisualStatusDateChange(VisualStatusBulkChangeRequest request, CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(request, cancellationToken);
            string cacheEntityName = $"Employee_{request.EmployeeId}";
            _memoryCache.Remove($"API::{cacheEntityName}");
            return Ok(response);
        }
        

        [HttpPost("VisualStatusDateChangeBulk")]
        public async Task<ActionResult<List<VisualStatusDateChangeBulkResponse>>> VisualStatusDateChangeBulk(VisualStatusDateChangeBulkRequest request, CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(request, cancellationToken);
            return Ok(response);
        }


        [HttpPost("ChangeRoomByDate")]
        //ChangeRoomByDateRequest
        public async Task<ActionResult> ChangeRoomByDate(ChangeRoomByDateRequest request, CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(request, cancellationToken);
            return Ok(response);
        }


        [HttpPost("ChangeRoomByDates")]
        public async Task<ActionResult<ChangeRoomByDatesResponse>> ChangeRoomByDates(ChangeRoomByDatesRequest request, CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(request, cancellationToken);
            return Ok(response);
        }



        [HttpGet("lasterstatus/onsite/{EmployeeId}/{EventDate}")]
        public async Task<ActionResult> LastOnsiteStatus(int EmployeeId, DateTime EventDate, CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(new DateLastEmployeeStatusRequest(EmployeeId, EventDate, 1), cancellationToken);
            return Ok(response);
        }

        [HttpGet("futurefirststatus/onsite/{EmployeeId}/{EventDate}")]
        public async Task<ActionResult> FutureOnsiteStatus(int EmployeeId, DateTime EventDate, CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(new DateLastEmployeeStatusRequest(EmployeeId, EventDate, 1, false), cancellationToken);
            return Ok(response);
        }



        [HttpGet("lasterstatus/offsite/{EmployeeId}/{EventDate}")]
        public async Task<ActionResult> LastOffSiteStatus(int EmployeeId, DateTime EventDate, CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(new DateLastEmployeeStatusRequest(EmployeeId, EventDate, 0), cancellationToken);
            return Ok(response);
        }


        [HttpGet("futurefirststatus/offsite/{EmployeeId}/{EventDate}")]
        public async Task<ActionResult> FutureOffSiteStatus(int EmployeeId, DateTime EventDate, CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(new DateLastEmployeeStatusRequest(EmployeeId, EventDate, 0, false), cancellationToken);
            return Ok(response);
        }

    }

}
