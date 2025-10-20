using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using tas.Application.Features.RequestTravelPurposeFeature.CreateRequestTravelPurpose;
using tas.Application.Features.RequestTravelPurposeFeature.DeleteRequestTravelPurpose;
using tas.Application.Features.RequestTravelPurposeFeature.GetAllRequestTravelPurpose;
using tas.Application.Features.RequestTravelPurposeFeature.UpdateRequestTravelPurpose;
using tas.Application.Service;

namespace tas.WebAPI.Controllers.Tas
{
    [Route("api/tas/[controller]")]
    [ApiController]
    [RoleAuthorize]
    public class RequestTravelPurposeController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<RequestTravelPurposeController> _logger;
        private readonly IMemoryCache _memoryCache;

        public RequestTravelPurposeController(IMediator mediator, ILogger<RequestTravelPurposeController> logger, IMemoryCache memoryCache)
        {
            _mediator = mediator;
            _logger = logger;
            _memoryCache = memoryCache;
        }

        [HttpGet]
        public async Task<ActionResult<List<GetAllRequestTravelPurposeResponse>>> GetAll(int? active, CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(new GetAllRequestTravelPurposeRequest(active), cancellationToken);
            return Ok(response);


        }

        [HttpPost]
        public async Task<ActionResult> Create(CreateRequestTravelPurposeRequest request,
            CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(request, cancellationToken);
            return Ok(response);
        }


        [HttpPut]
        public async Task<ActionResult> Update(UpdateRequestTravelPurposeRequest request,
            CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(request, cancellationToken);
            return Ok(response);
        }

        [HttpDelete]
        public async Task<ActionResult> Delete(DeleteRequestTravelPurposeRequest request,
            CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(request, cancellationToken);
            return Ok(response);
        }

    }
}
