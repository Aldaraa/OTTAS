using DocumentFormat.OpenXml.Office2010.Word;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using tas.Application.Features.MailSmtpConfigFeature.SendMailRequestDocument;
using tas.Application.Features.RequestAirportFeature.GetAllRequestAirport;
using tas.Application.Features.RequestDocumentFeature.CheckRequestDocumentSiteTravelAdd;
using tas.Application.Features.RequestDocumentFeature.CheckRequestDocumentSiteTravelRemove;
using tas.Application.Features.RequestDocumentFeature.CheckRequestDocumentSiteTravelReschedule;
using tas.Application.Features.RequestDocumentFeature.CompleteRequestDocumentSiteTravelAdd;
using tas.Application.Features.RequestDocumentFeature.CompleteRequestDocumentSiteTravelRemove;
using tas.Application.Features.RequestDocumentFeature.CompleteRequestDocumentSiteTravelReschedule;
using tas.Application.Features.RequestDocumentFeature.CreateRequestDocumentSiteTravelAdd;
using tas.Application.Features.RequestDocumentFeature.CreateRequestDocumentSiteTravelRemove;
using tas.Application.Features.RequestDocumentFeature.CreateRequestDocumentSiteTravelReschedule;
using tas.Application.Features.RequestDocumentFeature.GetRequestDocumentSiteTravelAdd;
using tas.Application.Features.RequestDocumentFeature.GetRequestDocumentSiteTravelRemove;
using tas.Application.Features.RequestDocumentFeature.GetRequestDocumentSiteTravelReschedule;
using tas.Application.Features.RequestDocumentFeature.UpdateRequestDocumentSiteTravelAdd;
using tas.Application.Features.RequestDocumentFeature.UpdateRequestDocumentSiteTravelRemove;
using tas.Application.Features.RequestDocumentFeature.UpdateRequestDocumentSiteTravelReschedule;
using tas.Application.Features.RequestDocumentProfileChangeFeature.CompleteRequestDocumentProfileChange;
using tas.Application.Features.RequestTravelAgentFeature.CreateRequestTravelAgent;
using tas.Application.Service;

namespace tas.WebAPI.Controllers.Tas
{


    [Route("api/tas/[controller]")]
    [ApiController]
    [RoleAuthorize]
    public class RequestSiteTravelController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<RequestSiteTravelController> _logger;

        public RequestSiteTravelController(IMediator mediator, ILogger<RequestSiteTravelController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }


        #region AddTravel

        [HttpPost("addtravel")]

        public async Task<ActionResult> CreateAdd(CreateRequestDocumentSiteTravelAddRequest request, CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(request, cancellationToken);
            if (!request.skipMailNotification)
            {
                await _mediator.Send(new SendMailRequestDocumentRequest(response), cancellationToken);
            }
            return Ok(response);
        }


        [HttpPut("addtravel")]

        public async Task<ActionResult> UpdateAdd(UpdateRequestDocumentSiteTravelAddRequest request, CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(request, cancellationToken);
            return Ok(response);
        }


        [HttpGet("addtravel/{documentId}")]
        public async Task<ActionResult> GetAdd(int documentId, CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(new GetRequestDocumentSiteTravelAddRequest(documentId), cancellationToken);
            return Ok(response);
        }


        [HttpPut("addtravel/complete/{documentId}")]
        public async Task<ActionResult> CompleteAdd(CompleteRequestDocumentSiteTravelAddRequest request,   CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(request, cancellationToken);
            await _mediator.Send(new SendMailRequestDocumentRequest(request.Id), cancellationToken);
            return Ok(response);
        }

        [HttpPost("addtravel/checkduplicate")]
        public async Task<ActionResult> CheckAdd(CheckRequestDocumentSiteTravelAddRequest request, CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(request, cancellationToken);
            return Ok(response);
        }

        #endregion


        #region ReScheduleTravel



        [HttpPost("reschedule")]

        public async Task<ActionResult> CreateReschedule(CreateRequestDocumentSiteTravelRescheduleRequest request, CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(request, cancellationToken);
            if (!request.skipMailNotification)
            {
                await _mediator.Send(new SendMailRequestDocumentRequest(response), cancellationToken);
            }
            return Ok(response);
        }




        [HttpPut("reschedule")]

        public async Task<ActionResult> UpdateReschedule(UpdateRequestDocumentSiteTravelRescheduleRequest request, CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(request, cancellationToken);
            return Ok(response);
        }



        [HttpGet("reschedule/{documentId}")]
        public async Task<ActionResult> GetReschedule(int documentId, CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(new GetRequestDocumentSiteTravelRescheduleRequest(documentId), cancellationToken);
            return Ok(response);
        }



        [HttpPut("reschedule/complete/{documentId}")]
        public async Task<ActionResult> CompleteReschedule(CompleteRequestDocumentSiteTravelRescheduleRequest request, CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(request, cancellationToken);
            await _mediator.Send(new SendMailRequestDocumentRequest(request.Id), cancellationToken);
            return Ok(response);
        }

        [HttpPost("reschedule/checkduplicate")]
        public async Task<ActionResult> CheckReschedule(CheckRequestDocumentSiteTravelRescheduleRequest request, CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(request, cancellationToken);
            return Ok(response);
        }

        #endregion


        #region RemoveTravel


        [HttpPost("removetravel")]

        public async Task<ActionResult> RemoveAdd(CreateRequestDocumentSiteTravelRemoveRequest request, CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(request, cancellationToken);
            if (!request.skipMailNotification)
            {
                await _mediator.Send(new SendMailRequestDocumentRequest(response), cancellationToken);
            }
            return Ok(response);
        }


        [HttpPut("removetravel")]

        public async Task<ActionResult> UpdateRemove(UpdateRequestDocumentSiteTravelRemoveRequest request, CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(request, cancellationToken);
            return Ok(response);
        }


        [HttpGet("remove/{documentId}")]
        public async Task<ActionResult> GetRemove(int documentId, CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(new GetRequestDocumentSiteTravelRemoveRequest(documentId), cancellationToken);
            return Ok(response);
        }


        [HttpPut("remove/complete/{documentId}")]
        public async Task<ActionResult> CompleteRemove(CompleteRequestDocumentSiteTravelRemoveRequest request, CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(request, cancellationToken);
            await _mediator.Send(new SendMailRequestDocumentRequest(request.Id), cancellationToken);
            return Ok(response);
        }


        [HttpPost("remove/checkduplicate")]
        public async Task<ActionResult> CheckRemove(CheckRequestDocumentSiteTravelRemoveRequest request, CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(request, cancellationToken);
            return Ok(response);
        }


        #endregion
    }

}
