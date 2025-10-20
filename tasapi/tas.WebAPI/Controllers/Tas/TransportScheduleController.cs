using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using tas.Application.Features.ColorFeature.GetAllColor;
using tas.Application.Features.ColorFeature.UpdateColor;
using tas.Application.Features.TransportScheduleFeature.BusstopTransportSchedule;
using tas.Application.Features.TransportScheduleFeature.CreateScheduleActiveTransport;
using tas.Application.Features.TransportScheduleFeature.CreateScheduleDriveTransport;
using tas.Application.Features.TransportScheduleFeature.GetDateDriveTransportSchedule;
using tas.Application.Features.TransportScheduleFeature.GetMonthTransportSchedule;
using tas.Application.Features.TransportScheduleFeature.GetScheduleBusstop;
using tas.Application.Features.TransportScheduleFeature.ManageTransportSchedule;
using tas.Application.Features.TransportScheduleFeature.RemoveScheduleBusstop;
using tas.Application.Features.TransportScheduleFeature.SeatInfoTransportSchedule;
using tas.Application.Features.TransportScheduleFeature.TransportScheduleExport;
using tas.Application.Features.TransportScheduleFeature.TransportScheduleInfo;
using tas.Application.Features.TransportScheduleFeature.UpdateDescription;
using tas.Application.Features.TransportScheduleFeature.UpdateScheduleBusstop;
using tas.Application.Features.TransportScheduleFeature.UpdateTransportSchedule;
using tas.Application.Features.TransportScheduleFeature.UpdateTransportScheduleRealETD;
using tas.Application.Features.TransportScheduleFeature.UpdateTransportScheduleRealETDByDate;
using tas.Application.Service;

namespace tas.WebAPI.Controllers.Tas
{
    [Route("api/tas/[controller]")]
    [ApiController]
    [RoleAuthorize]

    public class TransportScheduleController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<TransportScheduleController> _logger;

        public TransportScheduleController(IMediator mediator, ILogger<TransportScheduleController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }


        [HttpPut]
        public async Task<ActionResult> Update(UpdateTransportScheduleRequest request,
            CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(request, cancellationToken);
            return Ok(response);
        }


        [HttpPut("RealETD")]
        public async Task<ActionResult> UpdateRealETD(UpdateTransportScheduleRealETDRequest request,
    CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(request, cancellationToken);
            return Ok(response);
        }


        [HttpPut("RealETDbyDate")]
        public async Task<ActionResult> UpdateRealETDByDate(UpdateTransportScheduleRealETDByDateRequest request,
    CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(request, cancellationToken);
            return Ok(response);
        }


        [HttpGet("{Id}")]
        public async Task<ActionResult<TransportScheduleInfoResponse>> GetScheduleInfo(int Id,
    CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(new TransportScheduleInfoRequest(Id), cancellationToken);
            return Ok(response);
        }




        [HttpGet("busstop/{Id}")]
        public async Task<ActionResult<TransportScheduleInfoResponse>> GetBusstop(int Id,
    CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(new GetScheduleBusstopRequest(Id), cancellationToken);
            return Ok(response);
        }



        [HttpPost("monthtransportschedule")]
        public async Task<ActionResult> GetMonthTransportSchedule(GetMonthTransportScheduleRequest request,
            CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(request, cancellationToken);
            return Ok(response);
        }


        [HttpPost("createschedule")]
        public async Task<ActionResult> CreateSchedule(CreateScheduleActiveTransportRequest request,
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

        [HttpPost("createscheduledrive")]
        public async Task<ActionResult> CreateScheduleDrive(CreateScheduleDriveTransportRequest request,
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



        [HttpPost("manageschedule")]
        public async Task<ActionResult<ManageTransportScheduleResponse>> ManageSchedule(ManageTransportScheduleRequest request,
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


        [HttpPost("busstopschedule")]
        public async Task<ActionResult<BusstopTransportScheduleResponse>> BusstopSchedule(BusstopTransportScheduleRequest request,
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


        [HttpPut("description")]
        public async Task<ActionResult> UpdateScheduleDescription(UpdateDescriptionRequest request,
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


        [HttpPut("busstop")]
        public async Task<ActionResult> UpdateScheduleBusstop(UpdateScheduleBusstopRequest request,
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




        [HttpGet("seatinfo/{Id}")]
        public async Task<ActionResult<TransportScheduleInfoResponse>> GetSeatInfo(int Id,
    CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(new SeatInfoTransportScheduleRequest(Id), cancellationToken);
            return Ok(response);
        }




        [HttpPut("scheduleexport")]
        public async Task<ActionResult> TransportScheduleExport(TransportScheduleExportRequest request,
    CancellationToken cancellationToken)
        {

            var response = await _mediator.Send(request, cancellationToken);
            if (response.ExcelFile != null)
            {
                return File(response.ExcelFile, "application/force-download", $"TAS_TransportScheduleExport_Download_{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}.xlsx"); ;
            }
            else
            {
                return NoContent();
            }
        }



        [HttpDelete("busstop/{Id}")]
        public async Task<ActionResult<TransportScheduleInfoResponse>> DeleteBusstop(int Id,
CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(new RemoveScheduleBusstopRequest(Id), cancellationToken);
            return Ok(response);
        }





        [HttpPost("datedriveschedule")]
        public async Task<ActionResult<GetDateDriveTransportScheduleResponse>> BusstopSchedule(GetDateDriveTransportScheduleRequest request,
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



    }
}
