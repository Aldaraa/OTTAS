using DocumentFormat.OpenXml.Office2010.Word;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using tas.Application.Features.MailSmtpConfigFeature.SendMailRequestDocument;
using tas.Application.Features.RequestDocumentDeMobilisationFeature.CompleteRequestDocumentDeMobilisation;
using tas.Application.Features.RequestDocumentFeature.CompleteRequestDocumentNonSiteTravel;
using tas.Application.Features.RequestDocumentFeature.CreateRequestDocumentNonSiteTravel;
using tas.Application.Features.RequestDocumentFeature.CreateRequestDocumentSiteTravelAdd;
using tas.Application.Features.RequestDocumentFeature.GetRequestDocumentNonSiteTravel;
using tas.Application.Features.RequestDocumentFeature.GetRequestDocumentSiteTravelAdd;
using tas.Application.Features.RequestDocumentFeature.UpdateRequestDocumentNonSiteTravelData;
using tas.Application.Features.RequestDocumentFeature.UpdateRequestDocumentNonSiteTravelEmployee;
using tas.Application.Features.RequestDocumentFeature.WaitingAgentRequestDocumentNonSiteTravel;
using tas.Application.Service;

namespace tas.WebAPI.Controllers.Tas
{

    [Route("api/tas/[controller]")]
    [ApiController]
    [RoleAuthorize]
    public class RequestNonSiteTravelController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<RequestNonSiteTravelController> _logger;

        public RequestNonSiteTravelController(IMediator mediator, ILogger<RequestNonSiteTravelController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }


        [HttpPost]
        public async Task<ActionResult> CreateNonSiteTravel(CreateRequestDocumentNonSiteTravelRequest request, CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(request, cancellationToken);
            await _mediator.Send(new SendMailRequestDocumentRequest(response), cancellationToken);
            return Ok(response);
        }

        [HttpGet("{documentId}")]

        public async Task<ActionResult<GetRequestDocumentNonSiteTravelResponse>> Get(int documentId, CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(new GetRequestDocumentNonSiteTravelRequest(documentId), cancellationToken);
            return Ok(response);
        }


        [HttpPut("traveldata")]

        public async Task<ActionResult> UpdateTravleData(UpdateRequestDocumentNonSiteTravelDataRequest request, CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(request, cancellationToken);
            return Ok(response);
        }



        [HttpPut("employee")]
        public async Task<ActionResult> UpdateNonSiteTravelEmployeeRequest(UpdateRequestDocumentNonSiteTravelEmployeeRequest request, CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(request, cancellationToken);
            return Ok(response);
        }

        [HttpPut("complete/{documentId}")]
        public async Task<ActionResult> Complete(int documentId, string? comment, CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(new CompleteRequestDocumentNonSiteTravelRequest(documentId, comment), cancellationToken);
            await _mediator.Send(new SendMailRequestDocumentRequest(documentId), cancellationToken);
            return Ok(response);


        }


        [HttpPut("waitingagent/{documentId}")]
        public async Task<ActionResult> WaitingAgent(int documentId, string? comment, CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(new WaitingAgentRequestDocumentNonSiteTravelRequest(documentId, comment), cancellationToken);
            return Ok(response);
        }

    }
}
