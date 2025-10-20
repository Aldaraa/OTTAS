using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using tas.Application.Features.ActiveTransportFeature.CreateActiveTransport;
using tas.Application.Features.ActiveTransportFeature.CreateSpecialActiveTransport;
using tas.Application.Features.ActiveTransportFeature.DeleteActiveTransport;
using tas.Application.Features.ActiveTransportFeature.ExtendActiveTransport;
using tas.Application.Features.ActiveTransportFeature.GetAllActiveTransport;
using tas.Application.Features.ActiveTransportFeature.GetCalendarActiveTransport;
using tas.Application.Features.ActiveTransportFeature.GetDateActiveTransport;
using tas.Application.Features.ActiveTransportFeature.GetExtendActiveTransport;
using tas.Application.Features.ActiveTransportFeature.ScheduleListActiveTransport;
using tas.Application.Features.ActiveTransportFeature.UpdateActiveTransport;
using tas.Application.Features.ActiveTransportFeature.UpdateAircraftCodeActiveTransport;
using tas.Application.Features.ActiveTransportFeature.UpdateBusstopActiveTransport;
using tas.Application.Features.ActiveTransportFeature.UpdateDescrActiveTransport;
using tas.Application.Repositories;
using tas.Application.Service;

namespace tas.WebAPI.Controllers.Tas
{
    [Route("api/tas/[controller]")]
    [ApiController]
    [RoleAuthorize]
    public class ActiveTransportController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<ActiveTransportController> _logger;
        private readonly ITransportScheduleRepository _transportScheduleService;
        public ActiveTransportController(IMediator mediator, ILogger<ActiveTransportController> logger, ITransportScheduleRepository transportScheduleService)
        {
            _mediator = mediator;
            _logger = logger;
            _transportScheduleService = transportScheduleService;
        }


        [HttpPost("getactivetransports")]
        public async Task<ActionResult<List<GetAllActiveTransportResponse>>> GetAll(GetAllActiveTransportRequest request, CancellationToken cancellationToken)
        {
                var response = await _mediator.Send(request, cancellationToken);
                return Ok(response);
        }

        [HttpGet("schedule/{activeTransportId}")]
        public async Task<ActionResult<ScheduleListActiveTransportResponse>> Schedule(int activeTransportId, string? year,  CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(new ScheduleListActiveTransportRequest(activeTransportId, year), cancellationToken);
            return Ok(response);
        }

        [HttpGet("extendinfo/{activeTransportId}")]
        public async Task<ActionResult<GetExtendActiveTransportResponse>> GetExtendActiveTransport(int activeTransportId, CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(new GetExtendActiveTransportRequest(activeTransportId), cancellationToken);
            return Ok(response);
        }


        [HttpPost("extend")]
        public async Task<ActionResult> ExtendActiveTransport(ExtendActiveTransportRequest request, CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(request, cancellationToken);
            return Ok(response);
        }


        [HttpPost("createspecial")]
        public async Task<ActionResult> CreateSpecial(CreateSpecialActiveTransportRequest request,
     CancellationToken cancellationToken)
        {

            try
            {
                var response = await _mediator.Send(request, cancellationToken);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }


        [HttpGet("getdatetransport")]
        public async Task<ActionResult> GetDateTransport(DateTime eventDate, string Direction, int? fromLocationId, int? toLocationId,
            CancellationToken cancellationToken)
        {

            try
            {
                var response = await _mediator.Send(new GetDateActiveTransportRequest(eventDate, Direction, fromLocationId, toLocationId), cancellationToken);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }


        [HttpGet("getcalendardata")]
        public async Task<ActionResult> GetCalendarData(DateTime eventDate,
            CancellationToken cancellationToken)
        {
            try
            {
                var response = await _mediator.Send(new GetCalendarActiveTransportRequest(eventDate), cancellationToken);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }
        [HttpPut]
        public async Task<ActionResult> Update(UpdateActiveTransportRequest request,
            CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(request, cancellationToken);
            return Ok(response);
        }


        [HttpPut("changedescr")]
        public async Task<ActionResult> UpdateDescr(UpdateDescrActiveTransportRequest request,
    CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(request, cancellationToken);
            return Ok(response);
        }


        [HttpPut("aircraftcode")]
        public async Task<ActionResult> UpdateAircraftCode(UpdateAircraftCodeActiveTransportRequest request,
CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(request, cancellationToken);
            return Ok(response);
        }

        [HttpPut("busstop")]
        public async Task<ActionResult> UpdateBusstop(UpdateBusstopActiveTransportRequest request,
    CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(request, cancellationToken);
            return Ok(response);
        }

        [HttpDelete]
        public async Task<ActionResult> Delete(DeleteActiveTransportRequest request,
            CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(request, cancellationToken);
            return Ok(response);
        }

    }
}
