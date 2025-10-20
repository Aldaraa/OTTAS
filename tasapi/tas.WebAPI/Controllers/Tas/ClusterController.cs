using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using tas.Application.Features.ClusterFeature.CreateCluster;
using tas.Application.Features.ClusterFeature.DeleteCluster;
using tas.Application.Features.ClusterFeature.GetActiveTransportCluster;
using tas.Application.Features.ClusterFeature.GetAllCluster;
using tas.Application.Features.ClusterFeature.GetCluster;
using tas.Application.Features.ClusterFeature.UpdateCluster;
using tas.Application.Service;

namespace tas.WebAPI.Controllers.Tas
{
    [Route("api/tas/[controller]")]
    [ApiController]
    [RoleAuthorize]
    public class ClusterController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<ClusterController> _logger;

        public ClusterController(IMediator mediator, ILogger<ClusterController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }



        [HttpGet]
        public async Task<ActionResult<List<GetAllClusterResponse>>> GetAll(int? active, CancellationToken cancellationToken)
        {
            if (active == null)
            {
                var response = await _mediator.Send(new GetAllClusterRequest(null), cancellationToken);
                return Ok(response);
            }
            else
            {
                var response = await _mediator.Send(new GetAllClusterRequest(active), cancellationToken);
                return Ok(response);

            }
        }

        [HttpGet("{Id}")]
        public async Task<ActionResult<GetClusterResponse>> Get(int Id, CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(new GetClusterRequest(Id), cancellationToken);
            return Ok(response);
        }


        [HttpGet("activetransports/{Id}")]
        public async Task<ActionResult<List<GetAllClusterResponse>>> Activetransports(int Id, CancellationToken cancellationToken)
        {
                var response = await _mediator.Send(new GetActiveTransportClusterRequest(Id), cancellationToken);
                return Ok(response);
        }

        [HttpPost]
        public async Task<ActionResult> Create(CreateClusterRequest request,
            CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(request, cancellationToken);
            return Ok(response);
        }


        [HttpPut]
        public async Task<ActionResult> Update(UpdateClusterRequest request,
            CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(request, cancellationToken);
            return Ok(response);
        }

        [HttpDelete]
        public async Task<ActionResult> Delete(DeleteClusterRequest request,
            CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(request, cancellationToken);
            return Ok(response);
        }

    }
}
