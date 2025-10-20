using MediatR;
using Microsoft.AspNetCore.Mvc;
using tas.Application.Features.MailSmtpConfigFeature.SendMailRequestDocument;
using tas.Application.Features.RequestDocumentExternalTravelFeature.CompleteRequestDocumentExternalTravelAdd;
using tas.Application.Features.RequestDocumentExternalTravelFeature.CompleteRequestDocumentExternalTravelRemove;
using tas.Application.Features.RequestDocumentExternalTravelFeature.CompleteRequestDocumentExternalTravelReschedule;
using tas.Application.Features.RequestDocumentExternalTravelFeature.CreateRequestDocumentExternalTravelRemove;
using tas.Application.Features.RequestDocumentExternalTravelFeature.CreateRequestDocumentExternalTravelReschedule;
using tas.Application.Features.RequestDocumentExternalTravelFeature.CreateRequestExternalTravelAdd;
using tas.Application.Features.RequestDocumentExternalTravelFeature.GetRequestDocumentExternalTravelAdd;
using tas.Application.Features.RequestDocumentExternalTravelFeature.GetRequestDocumentExternalTravelRemove;
using tas.Application.Features.RequestDocumentExternalTravelFeature.GetRequestDocumentExternalTravelReschedule;
using tas.Application.Features.RequestDocumentExternalTravelFeature.UpdateRequestDocumentExternalTravelAdd;
using tas.Application.Features.RequestDocumentExternalTravelFeature.UpdateRequestDocumentExternalTravelReschedule;
using tas.Application.Features.RequestDocumentFeature.CompleteRequestDocumentSiteTravelAdd;
using tas.Application.Features.RequestDocumentFeature.CompleteRequestDocumentSiteTravelRemove;
using tas.Application.Features.RequestDocumentFeature.CompleteRequestDocumentSiteTravelReschedule;
using tas.Application.Features.RequestDocumentFeature.CreateRequestDocumentSiteTravelRemove;
using tas.Application.Features.RequestDocumentFeature.CreateRequestDocumentSiteTravelReschedule;
using tas.Application.Features.RequestDocumentFeature.GetRequestDocumentSiteTravelAdd;
using tas.Application.Features.RequestDocumentFeature.GetRequestDocumentSiteTravelRemove;
using tas.Application.Features.RequestDocumentFeature.GetRequestDocumentSiteTravelReschedule;
using tas.Application.Service;

namespace tas.WebAPI.Controllers.Tas
{
    [Route("api/tas/[controller]")]
    [ApiController]
    [RoleAuthorize]
    public class RequestExternalTravelController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<RequestExternalTravelController> _logger;

        public RequestExternalTravelController(IMediator mediator, ILogger<RequestExternalTravelController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }


        #region AddTravel

        [HttpPost("addtravel")]

        public async Task<ActionResult> CreateAdd(CreateRequestExternalTravelAddRequest request, CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(request, cancellationToken);
            return Ok(response);
        }


        [HttpPut("addtravel")]

        public async Task<ActionResult> UpdateAdd(UpdateRequestDocumentExternalTravelAddRequest request, CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(request, cancellationToken);
            return Ok(response);
        }


        [HttpGet("addtravel/{documentId}")]
        public async Task<ActionResult> GetAdd(int documentId, CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(new GetRequestDocumentExternalTravelAddRequest(documentId), cancellationToken);
            return Ok(response);
        }

        [HttpPut("addtravel/complete/{documentId}")]
        public async Task<ActionResult> CompleteAdd(int documentId, string? comment, CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(new CompleteRequestDocumentExternalTravelAddRequest(documentId, comment), cancellationToken);
            await _mediator.Send(new SendMailRequestDocumentRequest(documentId), cancellationToken);
            return Ok(response);
        }
        #endregion


        #region ReScheduleTravel



        [HttpPost("reschedule")]

        public async Task<ActionResult> CreateReschedule(CreateRequestDocumentExternalTravelRescheduleRequest request, CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(request, cancellationToken);
            return Ok(response);
        }



        [HttpPut("reschedule")]

        public async Task<ActionResult> UpdateReschedule(UpdateRequestDocumentExternalTravelRescheduleRequest request, CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(request, cancellationToken);
            return Ok(response);
        }


        [HttpGet("reschedule/{documentId}")]
        public async Task<ActionResult> GetReschedule(int documentId, CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(new GetRequestDocumentExternalTravelRescheduleRequest(documentId), cancellationToken);
            return Ok(response);
        }



        [HttpPut("reschedule/complete/{documentId}")]
        public async Task<ActionResult> CompleteReschedule(int documentId, string? comment, CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(new CompleteRequestDocumentExternalTravelRescheduleRequest(documentId, comment), cancellationToken);
            await _mediator.Send(new SendMailRequestDocumentRequest(documentId), cancellationToken);
            return Ok(response);
        }

        #endregion



        #region RemoveTravel


        [HttpPost("removetravel")]

        public async Task<ActionResult> RemoveAdd(CreateRequestDocumentExternalTravelRemoveRequest request, CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(request, cancellationToken);
            return Ok(response);
        }

        [HttpGet("remove/{documentId}")]
        public async Task<ActionResult> GetRemove(int documentId, CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(new GetRequestDocumentExternalTravelRemoveRequest(documentId), cancellationToken); ;
            return Ok(response);
        }



        [HttpPut("remove/complete/{documentId}")]
        public async Task<ActionResult> CompleteRemove(int documentId, string? comment, CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(new CompleteRequestDocumentExternalTravelRemoveRequest(documentId, comment), cancellationToken);
            await _mediator.Send(new SendMailRequestDocumentRequest(documentId), cancellationToken);
            return Ok(response);
        }


        #endregion

    }

}
