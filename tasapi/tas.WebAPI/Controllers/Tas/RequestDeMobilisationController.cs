using DocumentFormat.OpenXml.Office2010.Word;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using tas.Application.Features.MailSmtpConfigFeature.SendMailRequestDocument;
using tas.Application.Features.RequestDocumentDeMobilisationFeature.CompleteRequestDocumentDeMobilisation;
using tas.Application.Features.RequestDocumentDeMobilisationFeature.CreateRequestDocumentDeMobilisation;
using tas.Application.Features.RequestDocumentDeMobilisationFeature.GetRequestDocumentDeMobilisation;
using tas.Application.Features.RequestDocumentDeMobilisationFeature.UpdateRequestDocumentDeMobilisation;
using tas.Application.Features.RequestGroupFeature.GetAllRequestGroup;
using tas.Application.Service;

namespace tas.WebAPI.Controllers.Tas
{
    [Route("api/tas/[controller]")]
    [ApiController]
    [RoleAuthorize]
    public class RequestDeMobilisationController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<RequestDeMobilisationController> _logger;

        public RequestDeMobilisationController(IMediator mediator, ILogger<RequestDeMobilisationController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        [HttpGet("{documentId}")]
        public async Task<ActionResult<GetRequestDocumentDeMobilisationResponse>> Get(int documentId ,CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(new GetRequestDocumentDeMobilisationRequest(documentId), cancellationToken);

            return Ok(response);
        }


        [HttpPost]
        public async Task<ActionResult> Create(CreateRequestDocumentDeMobilisationRequest request, CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(request, cancellationToken);
            await _mediator.Send(new SendMailRequestDocumentRequest(response), cancellationToken);
            return Ok();


        }


        [HttpPut]
        public async Task<ActionResult> Update(UpdateRequestDocumentDeMobilisationRequest request, CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(request, cancellationToken);
            return Ok(response);


        }

        [HttpPut("complete/{documentId}")]
        public async Task<ActionResult> Complete(int documentId, string? comment, CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(new CompleteRequestDocumentDeMobilisationRequest(documentId, comment), cancellationToken);
            await _mediator.Send(new SendMailRequestDocumentRequest(documentId), cancellationToken);
            return Ok(response);


        }
    }


}