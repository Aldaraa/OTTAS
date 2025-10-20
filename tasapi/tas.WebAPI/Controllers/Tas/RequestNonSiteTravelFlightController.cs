using MediatR;
using Microsoft.AspNetCore.Mvc;
using tas.Application.Features.MailSmtpConfigFeature.SendMailRequestNonsiteDocument;
using tas.Application.Features.RequestGroupConfigFeature.GetRequestDocumentType;
using tas.Application.Features.RequestNonSiteTravelFlightFeature.CreateRequestNonSiteTravelFlight;
using tas.Application.Features.RequestNonSiteTravelFlightFeature.DeleteRequestNonSiteTravelFlight;
using tas.Application.Features.RequestNonSiteTravelFlightFeature.UpdateRequestNonSiteTravelFlight;
using tas.Application.Service;

namespace tas.WebAPI.Controllers.Tas
{


    [Route("api/tas/[controller]")]
    [ApiController]
    [RoleAuthorize]
    public class RequestNonSiteTravelFlightController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<RequestNonSiteTravelFlightController> _logger;

        public RequestNonSiteTravelFlightController(IMediator mediator, ILogger<RequestNonSiteTravelFlightController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

  

        [HttpPost]
        public async Task<ActionResult> CreateRequestNonSiteTravelFlight(CreateRequestNonSiteTravelFlightRequest request, CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(request, cancellationToken);
            await _mediator.Send(new SendMailRequestNonsiteDocumentRequest(request.DocumentId, "Flight added"), cancellationToken);
            return Ok(response);
        }

        [HttpPut]
        public async Task<ActionResult> UpdateRequestNonSiteTravelFlight(UpdateRequestNonSiteTravelFlightRequest request, CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(request, cancellationToken);
            await _mediator.Send(new SendMailRequestNonsiteDocumentRequest(request.DocumentId, "Flight updated"), cancellationToken);
            return Ok(response);
        }


        [HttpDelete("{Id}")]
        public async Task<ActionResult> DeleteRequestNonSiteTravelFlight(int Id, CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(new DeleteRequestNonSiteTravelFlightRequest(Id), cancellationToken);
            return Ok(response);
        }


    }

}
