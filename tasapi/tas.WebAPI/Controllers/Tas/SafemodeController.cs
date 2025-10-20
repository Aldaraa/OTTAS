using MediatR;
using Microsoft.AspNetCore.Mvc;
using tas.Application.Features.SafeModeEmployeeStatusFeature.CreateEmployeeStatus;
using tas.Application.Features.SafeModeEmployeeStatusFeature.GetEmployeeStatus;
using tas.Application.Features.SafeModeEmployeeStatusFeature.SetDSEmployeeStatus;
using tas.Application.Features.SafeModeEmployeeStatusFeature.SetRREmployeeStatus;
using tas.Application.Features.SafeModeTransportFeature.CreateTransport;
using tas.Application.Features.SafeModeTransportFeature.DeleteTransport;
using tas.Application.Features.SafeModeTransportFeature.UpdateTransport;
using tas.Application.Features.TransportFeature.AddTravelTransport;
using tas.Application.Features.TransportModeFeature.DeleteTransportMode;
using tas.Application.Service;

namespace tas.WebAPI.Controllers.Tas
{
    [Route("api/tas/[controller]")]
    [ApiController]
    [RoleAuthorize]
    public class SafemodeController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<SafemodeController> _logger;
        private readonly CacheService _memoryCache;

        public SafemodeController(IMediator mediator, ILogger<SafemodeController> logger, CacheService memoryCache)
        {
            _mediator = mediator;
            _logger = logger;
            _memoryCache = memoryCache;
        }


        [HttpPost("transport")]
        public async Task<ActionResult> CreateTransport(CreateTransportRequest request, CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(request, cancellationToken);
            string cacheEntityName = $"Employee_{request.EmployeeId}";
            _memoryCache.Remove($"API::{cacheEntityName}");
            return Ok(response);
        }

        [HttpPut("transport")]
        public async Task<ActionResult> UpdateTransport(UpdateTransportRequest request, CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(request, cancellationToken);
            return Ok(response);
        }


        [HttpDelete("transport/{Id}")]
        public async Task<ActionResult> DeleteTransport(int Id, CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(new DeleteTransportRequest(Id), cancellationToken);
            return Ok(response);
        }


        [HttpPost("employeestatus")]
        public async Task<ActionResult> CreateEmployeeStatus(CreateEmployeeStatusRequest request, CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(request, cancellationToken);
            string cacheEntityName = $"Employee_{request.EmployeeId}";
            _memoryCache.Remove($"API::{cacheEntityName}");
            return Ok(response);
        }

        [HttpGet("employeestatus/{EmployeeId}/{EventDate}")]
        public async Task<ActionResult> GetEmployeeStatus(int EmployeeId, DateTime EventDate, CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(new GetEmployeeStatusRequest(EmployeeId, EventDate), cancellationToken);
            string cacheEntityName = $"Employee_{EmployeeId}";
            _memoryCache.Remove($"API::{cacheEntityName}");
            return Ok(response);
        }


        [HttpPut("setrr")]
        public async Task<ActionResult> UpdateRREmployeeStatus(SetRREmployeeStatusRequest request, CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(request, cancellationToken);
            return Ok(response);
        }



        [HttpPut("setds")]
        public async Task<ActionResult> UpdateDSEmployeeStatus(SetDSEmployeeStatusRequest request, CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(request, cancellationToken);
            return Ok(response);
        }


    }

}
