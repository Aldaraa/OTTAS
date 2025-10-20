using DocumentFormat.OpenXml.Office2016.Excel;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using tas.Application.Features.MailSmtpConfigFeature.SendMailRequestDocument;
using tas.Application.Features.RequestDocumentProfileChangeFeature.CompleteRequestDocumentProfileChange;
using tas.Application.Features.RequestDocumentProfileChangeFeature.CompleteRequestDocumentProfileChangeTemp;
using tas.Application.Features.RequestDocumentProfileChangeFeature.CreateRequestDocumentProfileChange;
using tas.Application.Features.RequestDocumentProfileChangeFeature.CreateRequestDocumentProfileChangeTemp;
using tas.Application.Features.RequestDocumentProfileChangeFeature.GetRequestDocumentProfileChange;
using tas.Application.Features.RequestDocumentProfileChangeFeature.GetRequestDocumentProfileChangeTemp;
using tas.Application.Features.RequestDocumentProfileChangeFeature.UpdateRequestDocumentProfileChange;
using tas.Application.Features.RequestDocumentProfileChangeFeature.UpdateRequestDocumentProfileChangeTemp;
using tas.Application.Features.RequestGroupEmployeeFeature.RequestGroupActiveEmployees;
using tas.Application.Service;

namespace tas.WebAPI.Controllers.Tas
{

    [Route("api/tas/[controller]")]
    [ApiController]
    [RoleAuthorize]
    public class RequestDocumentProfileChangeController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<RequestDocumentProfileChangeController> _logger;

        public RequestDocumentProfileChangeController(IMediator mediator, ILogger<RequestDocumentProfileChangeController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        [HttpGet("{Id}")]
        public async Task<ActionResult<GetRequestDocumentProfileChangeResponse>> GetDocumentProfileChange(int Id, CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(new GetRequestDocumentProfileChangeRequest(Id), cancellationToken);
            return Ok(response);
        }

        [HttpGet("temp/{Id}")]
        public async Task<ActionResult<GetRequestDocumentProfileChangeResponse>> GetDocumentProfileChangeTemp(int Id, CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(new GetRequestDocumentProfileChangeTempRequest(Id), cancellationToken);
            return Ok(response);
        }



        [HttpPut]
        public async Task<ActionResult> UpdateDocumentProfileChange(UpdateRequestDocumentProfileChangeRequest request,  CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(request, cancellationToken);
            return Ok(response);
        }


        [HttpPut("temp")]
        public async Task<ActionResult> UpdateDocumentProfileChangeTemp(UpdateRequestDocumentProfileChangeTempRequest request, CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(request, cancellationToken);
            return Ok(response);
        }



        [HttpPost]
        public async Task<ActionResult> CreateDocumentProfileChange(CreateRequestDocumentProfileChangeRequest request, CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(request, cancellationToken);
            await _mediator.Send(new SendMailRequestDocumentRequest(response), cancellationToken);
            return Ok(response);
        }

        [HttpPost("temp")]
        public async Task<ActionResult> CreateDocumentProfileChangeTemp(CreateRequestDocumentProfileChangeTempRequest request, CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(request, cancellationToken);
            await _mediator.Send(new SendMailRequestDocumentRequest(response), cancellationToken);
            return Ok(response);
        }




        [HttpPut("complete/{documentId}")]
        public async Task<ActionResult> CompleteDocumentProfileChange(int documentId, string? comment, CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(new CompleteRequestDocumentProfileChangeRequest(documentId, comment), cancellationToken);
            await _mediator.Send(new SendMailRequestDocumentRequest(documentId), cancellationToken);
            return Ok(response);
        }



        [HttpPut("complete/temp/{documentId}")]
        public async Task<ActionResult> CompleteDocumentProfileChangeTemp(int documentId, string? comment,   CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(new CompleteRequestDocumentProfileChangeTempRequest(documentId, comment), cancellationToken);
            await _mediator.Send(new SendMailRequestDocumentRequest(documentId), cancellationToken);
            return Ok(response);
        }
    }
}
