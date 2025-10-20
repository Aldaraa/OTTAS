using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using tas.Application.Features.MailSmtpConfigFeature.SendMailRequestNonsiteDocument;
using tas.Application.Features.RequestNonSiteTravelOptionDataFeature.UpdateRequestNonSiteTravelOptionData;
using tas.Application.Features.RequestNonSiteTravelOptionFeature.CreateRequestNonSiteTravelOption;
using tas.Application.Features.RequestNonSiteTravelOptionFeature.DeleteRequestNonSiteTravelOption;
using tas.Application.Features.RequestNonSiteTravelOptionFeature.GetRequestNonSiteTravelOption;
using tas.Application.Features.RequestNonSiteTravelOptionFeature.GetRequestNonSiteTravelOptionFinal;
using tas.Application.Features.RequestNonSiteTravelOptionFeature.UpdateItineraryOption;
using tas.Application.Features.RequestNonSiteTravelOptionFeature.UpdateRequestNonSiteTravelOption;
using tas.Application.Features.RequestTravelAgentFeature.CreateRequestTravelAgent;
using tas.Application.Service;

namespace tas.WebAPI.Controllers.Tas
{

    [Route("api/tas/[controller]")]
    [ApiController]
    [RoleAuthorize]
    public class RequestNonSiteTravelOptionController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<RequestNonSiteTravelOptionController> _logger;
        private readonly IMemoryCache _memoryCache;

        public RequestNonSiteTravelOptionController(IMediator mediator, ILogger<RequestNonSiteTravelOptionController> logger, IMemoryCache memoryCache)
        {
            _mediator = mediator;
            _logger = logger;
            _memoryCache = memoryCache;
        }



        [HttpPost]
        public async Task<ActionResult> Create(CreateRequestNonSiteTravelOptionRequest request,
            CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(request, cancellationToken);
            await _mediator.Send(new SendMailRequestNonsiteDocumentRequest(request.DocumentId, "Ticket option updated"), cancellationToken);
            return Ok(response);
        }


        [HttpPost("UpdateItinerary")]
        public async Task<ActionResult> UpdateItinerary(UpdateItineraryOptionRequest request,
    CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(request, cancellationToken);
            return Ok(response);
        }



        [HttpPut]
        public async Task<ActionResult> Update(UpdateRequestNonSiteTravelOptionRequest request,
    CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(request, cancellationToken);
            return Ok(response);
        }





        [HttpPut("fulldata")]
        public async Task<ActionResult> UpdateFull(UpdateRequestNonSiteTravelOptionDataRequest request,
    CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(request, cancellationToken);
            return Ok(response);
        }



        [HttpGet("{documentId}")]
        public async Task<ActionResult> Get(int documentId,
    CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(new GetRequestNonSiteTravelOptionRequest(documentId), cancellationToken);
            return Ok(response);
        }

        [HttpGet("final/{documentId}")]
        public async Task<ActionResult> GetFinalOptionData(int documentId,
CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(new GetRequestNonSiteTravelOptionFinalRequest(documentId), cancellationToken);
            return Ok(response);
        }



        [HttpDelete("{Id}")]
        public async Task<ActionResult> Delete(int Id,
    CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(new DeleteRequestNonSiteTravelOptionRequest(Id), cancellationToken);
            return Ok(response);
        }


    }
}
