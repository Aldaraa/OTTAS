using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using tas.Application.Features.StatusChangesEmployeeRequestFeature.DeleteStatusChangesEmployeeRequest;
using tas.Application.Features.StatusChangesEmployeeRequestFeature.GetStatusChangesEmployeeRequestDeActive;
using tas.Application.Features.StatusChangesEmployeeRequestFeature.GetStatusChangesEmployeeRequestReActive;
using tas.Application.Service;
using tas.Domain.Common;

namespace tas.WebAPI.Controllers.Tas
{


    [Route("api/tas/[controller]")]
    [ApiController]
    [RoleAuthorize]
    public class StatusChangesEmployeeRequestController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<StatusChangesEmployeeRequestController> _logger;
        private readonly IMemoryCache _memoryCache;


        public StatusChangesEmployeeRequestController(IMediator mediator, ILogger<StatusChangesEmployeeRequestController> logger, IMemoryCache memoryCache)
        {
            _mediator = mediator;
            _logger = logger;
            _memoryCache = memoryCache;
        }

        [HttpGet("deactive")]
        public async Task<ActionResult<List<GetStatusChangesEmployeeRequestDeActiveResponse>>> GetDeactiveRequest(CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(new GetStatusChangesEmployeeRequestDeActiveRequest(), cancellationToken);
            return Ok(response);
        }

        [HttpGet("reactive")]
        public async Task<ActionResult<List<GetStatusChangesEmployeeRequestDeActiveResponse>>> GetReActiveRequest(CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(new GetStatusChangesEmployeeRequestReActiveRequest(), cancellationToken);
            return Ok(response);
        }

        [HttpDelete("{Id}")]
        public async Task<ActionResult<List<GetStatusChangesEmployeeRequestDeActiveResponse>>> DeleteStatusRequest(int Id, CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(new DeleteStatusChangesEmployeeRequestRequest(Id), cancellationToken);
            return Ok(response);
        }
    }

}