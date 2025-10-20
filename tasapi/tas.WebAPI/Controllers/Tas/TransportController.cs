using DocumentFormat.OpenXml.Office2010.Excel;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using tas.Application.Features.CostCodeFeature.GetAllCostCode;
using tas.Application.Features.MultipleBookingFeature.MultipleBookingAddTransport;
using tas.Application.Features.MultipleBookingFeature.MultipleBookingPreviewTransport;
using tas.Application.Features.TransportFeature.AddExternalTravel;
using tas.Application.Features.TransportFeature.AddTravelTransport;
using tas.Application.Features.TransportFeature.CheckDataRequest;
using tas.Application.Features.TransportFeature.CreateNoGoShow;
using tas.Application.Features.TransportFeature.DeleteNoGoShow;
using tas.Application.Features.TransportFeature.DeleteScheduleTransport;
using tas.Application.Features.TransportFeature.EmployeeDateTransport;
using tas.Application.Features.TransportFeature.EmployeeExistingTransport;
using tas.Application.Features.TransportFeature.EmployeeTransportGoShow;
using tas.Application.Features.TransportFeature.EmployeeTransportNoShow;
using tas.Application.Features.TransportFeature.GetDataRequest;
using tas.Application.Features.TransportFeature.GetEmployeeTransport;
using tas.Application.Features.TransportFeature.GetEmployeeTransportAll;
using tas.Application.Features.TransportFeature.GetScheduleDetailTransport;
using tas.Application.Features.TransportFeature.RemoveExternalTransport;
using tas.Application.Features.TransportFeature.RemoveTransport;
using tas.Application.Features.TransportFeature.ReScheduleExternalTransport;
using tas.Application.Features.TransportFeature.ReScheduleMultiple;
using tas.Application.Features.TransportFeature.ReScheduleUpdate;
using tas.Application.Features.TransportFeature.SearchReSchedulePeople;
using tas.Application.Features.TransportFeature.TransportBookingInfo;
using tas.Application.Service;
using tas.Domain.Entities;

namespace tas.WebAPI.Controllers.Tas
{
    [Route("api/tas/[controller]")]
    [ApiController]
    [RoleAuthorize]
    public class TransportController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<TransportController> _logger;
        private readonly CacheService _memoryCache;

        public TransportController(IMediator mediator, ILogger<TransportController> logger, CacheService memoryCache)
        {
            _mediator = mediator;
            _logger = logger;
            _memoryCache = memoryCache;

        }

        [HttpGet]
        public async Task<ActionResult<List<GetEmployeeTransportResponse>>> EmpoyeeTransport(int employeeId, DateTime? startDate, DateTime endDate, CancellationToken cancellationToken)
        {

            var response = await _mediator.Send(new GetEmployeeTransportRequest(employeeId, startDate, endDate), cancellationToken);
            return Ok(response);
        }
        [HttpGet("all/{employeeId}/{startDate}")]
        public async Task<ActionResult<List<GetEmployeeTransportResponse>>> EmpoyeeTransport(int employeeId, DateTime startDate, CancellationToken cancellationToken)
        {

            var response = await _mediator.Send(new GetEmployeeTransportAllRequest(employeeId, startDate), cancellationToken);;
            return Ok(response);
        }


        [HttpGet("scheduleDetail")]
        public async Task<ActionResult<List<GetScheduleDetailTransportResponse>>> ScheduleDetail(int ScheduleId, CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(new GetScheduleDetailTransportRequest(ScheduleId), cancellationToken);
            return Ok(response);
        }


        [HttpPost("addtravel")]
        public async Task<ActionResult> AddTravel(AddTravelTransportRequest request, CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(request, cancellationToken);
            string cacheEntityName = $"Employee_{request.EmployeeId}";
            _memoryCache.Remove($"API::{cacheEntityName}");
            return Ok(response);
        }


        [HttpPost("addexternaltravel")]
        public async Task<ActionResult> AddExternalTravel(AddExternalTravelRequest request, CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(request, cancellationToken);
            string cacheEntityName = $"Employee_{request.EmployeeId}";
            _memoryCache.Remove($"API::{cacheEntityName}");
            return Ok(response);
        }

        [HttpDelete("schedule")]
        public async Task<ActionResult> DeleteSchedule(DeleteScheduleTransportRequest request, CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(request, cancellationToken);
            return Ok(response);
        }

        [HttpPost("searchreschedulepeople")]
        public async Task<ActionResult<List<SearchReSchedulePeopleRequest>>> SearchReschuleData(SearchReSchedulePeopleRequest request, CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(request, cancellationToken);
            return Ok(response);
        }

        [HttpPost("reschedule")]
        public async Task<ActionResult> ReScheduleMuplitiple(ReScheduleMultipleRequest request, CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(request, cancellationToken);
            return Ok(response);
        }



        [HttpPost("dateschedule")]
        public async Task<ActionResult> EmployeeDateTransportSchedule(EmployeeDateTransportRequest request , CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(request, cancellationToken);
            return Ok(response);
        }


        [HttpDelete("removetransport")]
        public async Task<ActionResult> EmployeeDateTransportSchedule(RemoveTransportRequest request, CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(request, cancellationToken);
            return Ok(response);
        }


        [HttpDelete("removeexternaltransport")]
        public async Task<ActionResult> RemoveExternalTransport(RemoveExternalTransportRequest request, CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(request, cancellationToken);
            return Ok(response);
        }


        [HttpPut("rescheduleexternaltransport")]
        public async Task<ActionResult> ReScheduleExternalTransport(ReScheduleExternalTransportRequest request, CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(request, cancellationToken);
            return Ok(response);
        }

        [HttpGet("noshow/{empId}/{year}")]
        public async Task<ActionResult> EmployeeTransportNoShow(int empId, int year , CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(new EmployeeTransportNoShowRequest(empId, year), cancellationToken);
            return Ok(response);
        }


        [HttpGet("goshow/{empId}/{year}")]
        public async Task<ActionResult> EmployeeTransportGoShow(int empId, int year, CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(new EmployeeTransportGoShowRequest(empId, year), cancellationToken);
            return Ok(response);
        }

        [HttpPost("getdatacheck")]
        public async Task<ActionResult> GetDataRequestRequest(GetDataRequestRequest request, CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(request, cancellationToken);
            return Ok(response);
        }


        [HttpPut("getdatacheck")]
        public async Task<ActionResult> CheckDataRequestRequest(CheckDataRequestRequest request, CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(request, cancellationToken);
            return Ok(response);
        }


        [HttpPost("bookinginfo")]
        public async Task<ActionResult> BookingInfo(TransportBookingInfoRequest request, CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(request, cancellationToken);
            return Ok(response);
        }



        [HttpPut("reschedule")]
        public async Task<ActionResult> ReScheduleUpdate(ReScheduleUpdateRequest request, CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(request, cancellationToken);
            return Ok(response);
        }


        [HttpGet("employee/existingtransport/{employeeId}/{startDate}/{endDate}")]
        public async Task<ActionResult> EmployeeExistingBooking(int employeeId, DateTime startDate, DateTime endDate, CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(new EmployeeExistingTransportRequest(employeeId, startDate, endDate), cancellationToken);
            return Ok(response);
        }



        [HttpPost("nogoshow")]
        public async Task<ActionResult> CreateGoShow(CreateNoGoShowRequest request, CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(request, cancellationToken);
            return Ok(response);
        }



        [HttpDelete("goshow/{id}")]
        public async Task<ActionResult> DeleteGoShow(int id, CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(new DeleteNoGoShowRequest(id, false), cancellationToken);
            return Ok(response);
        }

        [HttpDelete("noshow/{id}")]
        public async Task<ActionResult> DeleteNoShow(int id, CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(new DeleteNoGoShowRequest(id, true), cancellationToken);
            return Ok(response);
        }

        //MultipleBookingPreviewTransportRequest


        [HttpPost("multiplebooking/preview")]
        public async Task<ActionResult> MultipleBookingPreviewTransport(MultipleBookingPreviewTransportRequest request, CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(request, cancellationToken);
            return Ok(response);
        }


        [HttpPost("multiplebooking")]
        public async Task<ActionResult> MultipleBookingAddTransport(MultipleBookingAddTransportRequest request, CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(request, cancellationToken);
            return Ok(response);
        }

    }
}
