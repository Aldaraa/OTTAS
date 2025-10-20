using MediatR;
using Microsoft.AspNetCore.Mvc;
using tas.Application.Features.MailSmtpConfigFeature.SendMailRequestNonsiteDocument;
using tas.Application.Features.RequestNonSiteTravelAccommodationFeature.CreateRequestNonSiteTravelAccommodation;
using tas.Application.Features.RequestNonSiteTravelAccommodationFeature.DeleteRequestNonSiteTravelAccommodation;
using tas.Application.Features.RequestNonSiteTravelAccommodationFeature.UpdateRequestNonSiteTravelAccommodation;
using tas.Application.Service;

namespace tas.WebAPI.Controllers.Tas
{
    [Route("api/tas/[controller]")]
    [ApiController]
    [RoleAuthorize]
    public class RequestNonSiteTravelAccommodationController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<RequestNonSiteTravelAccommodationController> _logger;

        public RequestNonSiteTravelAccommodationController(IMediator mediator, ILogger<RequestNonSiteTravelAccommodationController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }



        [HttpPost]
        public async Task<ActionResult> CreateRequestNonSiteTravelAccommodation(CreateRequestNonSiteTravelAccommodationRequest request, CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(request, cancellationToken);
            await _mediator.Send(new SendMailRequestNonsiteDocumentRequest(request.DocumentId, "Accommodation added"), cancellationToken);
            return Ok(response);
        }

        [HttpPut]
        public async Task<ActionResult> UpdateRequestNonSiteTravelAccommodation(UpdateRequestNonSiteTravelAccommodationRequest request, CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(request, cancellationToken);
            await _mediator.Send(new SendMailRequestNonsiteDocumentRequest(request.DocumentId, "Accommodation updated"), cancellationToken);
            return Ok(response);
        }


        [HttpDelete("{Id}")]
        public async Task<ActionResult> DeleteRequestNonSiteTravelAccommodation(int Id, CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(new DeleteRequestNonSiteTravelAccommodationRequest(Id), cancellationToken);
            return Ok(response);
        }


    }
}
