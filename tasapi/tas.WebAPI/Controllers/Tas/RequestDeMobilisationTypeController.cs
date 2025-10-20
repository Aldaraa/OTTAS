using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using tas.Application.Features.RequestDeMobilisationTypeFeature.CreateRequestDeMobilisationType;
using tas.Application.Features.RequestDeMobilisationTypeFeature.DeleteRequestDeMobilisationType;
using tas.Application.Features.RequestDeMobilisationTypeFeature.GetAllRequestDeMobilisationType;
using tas.Application.Features.RequestDeMobilisationTypeFeature.UpdateRequestDeMobilisationType;
using tas.Application.Service;
using tas.Domain.Common;
using tas.Domain.Entities;

namespace tas.WebAPI.Controllers.Tas
{
    [Route("api/tas/[controller]")]
    [ApiController]
    [RoleAuthorize]
    public class RequestDeMobilisationTypeController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<RequestDeMobilisationTypeController> _logger;
        private readonly IMemoryCache _memoryCache;

        public RequestDeMobilisationTypeController(IMediator mediator, ILogger<RequestDeMobilisationTypeController> logger, IMemoryCache memoryCache)
        {
            _mediator = mediator;
            _logger = logger;
            _memoryCache = memoryCache;
        }

        [HttpGet]
        public async Task<ActionResult<List<GetAllRequestDeMobilisationTypeResponse>>> GetAll(int? active, CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(new GetAllRequestDeMobilisationTypeRequest(active), cancellationToken);
            return Ok(response);


        }

        [HttpPost]
        public async Task<ActionResult> Create(CreateRequestDeMobilisationTypeRequest request,
            CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(request, cancellationToken);
            return Ok(response);
        }


        [HttpPut]
        public async Task<ActionResult> Update(UpdateRequestDeMobilisationTypeRequest request,
            CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(request, cancellationToken);
            return Ok(response);
        }

        [HttpDelete]
        public async Task<ActionResult> Delete(DeleteRequestDeMobilisationTypeRequest request,
            CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(request, cancellationToken);
            return Ok(response);
        }

    }
}
