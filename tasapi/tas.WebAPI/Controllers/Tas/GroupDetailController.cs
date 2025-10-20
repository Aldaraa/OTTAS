using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using tas.Application.Features.GroupDetailFeature.CreateGroupDetail;
using tas.Application.Features.GroupDetailFeature.DeleteGroupDetail;
using tas.Application.Features.GroupDetailFeature.GetAllGroupDetail;
using tas.Application.Features.GroupDetailFeature.UpdateGroupDetail;
using tas.Application.Service;

namespace tas.WebAPI.Controllers.Tas
{
    [Route("api/tas/[controller]")]
    [ApiController]
    [RoleAuthorize]
    public class GroupDetailController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<GroupDetailController> _logger;

        public GroupDetailController(IMediator mediator, ILogger<GroupDetailController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }


        [HttpGet("{GroupMasterId}")]
        public async Task<ActionResult<GetAllGroupDetailResponse>> GetAll(int? active, int GroupMasterId, CancellationToken cancellationToken)
        {
            if (active == null)
            {
                var response = await _mediator.Send(new GetAllGroupDetailRequest(null, GroupMasterId), cancellationToken);
                return Ok(response);
            }
            else
            {
                var response = await _mediator.Send(new GetAllGroupDetailRequest(active, GroupMasterId), cancellationToken);
                return Ok(response);

            }
        }

        [HttpPost]
        public async Task<ActionResult> Create(CreateGroupDetailRequest request,
            CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(request, cancellationToken);
            return Ok(response);
        }


        [HttpPut]
        public async Task<ActionResult> Update(UpdateGroupDetailRequest request,
            CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(request, cancellationToken);
            return Ok(response);
        }

        [HttpDelete]
        public async Task<ActionResult> Delete(DeleteGroupDetailRequest request,
            CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(request, cancellationToken);
            return Ok(response);
        }

    }
}
