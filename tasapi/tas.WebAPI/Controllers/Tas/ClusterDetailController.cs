using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using tas.Application.Features.ClusterDetailFeature.CreateClusterDetail;
using tas.Application.Features.ClusterDetailFeature.DeleteClusterDetail;
using tas.Application.Features.ClusterDetailFeature.ReOrderClusterDetail;
using tas.Application.Service;

namespace tas.WebAPI.Controllers.Tas
{
    [Route("api/tas/[controller]")]
    [ApiController]
    [RoleAuthorize]
    public class ClusterDetailController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<ClusterDetailController> _logger;

        public ClusterDetailController(IMediator mediator, ILogger<ClusterDetailController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        [HttpPost]
        public async Task<ActionResult> Create(CreateClusterDetailRequest request,
            CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(request, cancellationToken);
            return Ok(response);
        }

        [HttpPost("reorder")]
        public async Task<ActionResult> ReOrder(ReOrderClusterDetailRequest request,
            CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(request, cancellationToken);
            return Ok(response);
        }


        [HttpDelete("{Id}")]
        public async Task<ActionResult> Delete(int Id,
            CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(new DeleteClusterDetailRequest(Id), cancellationToken);
            return Ok(response);
        }


    }
}
