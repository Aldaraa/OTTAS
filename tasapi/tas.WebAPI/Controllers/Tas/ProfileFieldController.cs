using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using tas.Application.Features.ProfileFieldFeature.GetAllProfileField;
using tas.Application.Features.ProfileFieldFeature.UpdateProfileField;
using tas.Application.Features.RequestAirportFeature.GetAllRequestAirport;
using tas.Application.Service;

namespace tas.WebAPI.Controllers.Tas
{

    [Route("api/tas/[controller]")]
    [ApiController]
    [RoleAuthorize]
    public class ProfileFieldController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<ProfileFieldController> _logger;
        private readonly IMemoryCache _memoryCache;

        public ProfileFieldController(IMediator mediator, ILogger<ProfileFieldController> logger, IMemoryCache memoryCache)
        {
            _mediator = mediator;
            _logger = logger;
            _memoryCache = memoryCache;
        }


        [HttpGet]
        public async Task<ActionResult<List<GetAllProfileFieldResponse>>> GetAll(CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(new GetAllProfileFieldRequest(), cancellationToken);
            return Ok(response);
        }


        [HttpPut]
        public async Task<ActionResult> UpdateProfileField(UpdateProfileFieldRequest request, CancellationToken  cancellationToken)
        {
            var response = await _mediator.Send(request, cancellationToken);
            return Ok(response);
        }

    }

}
