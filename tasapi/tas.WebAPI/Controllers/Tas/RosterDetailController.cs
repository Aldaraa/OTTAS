using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using tas.Application.Features.RosterDetailFeature.CreateRoster;
using tas.Application.Features.RosterDetailFeature.DeleteRosterDetail;
using tas.Application.Features.RosterDetailFeature.GetRosterDetail;
using tas.Application.Features.RosterDetailFeature.UpdateRosterDetail;
using tas.Application.Features.RosterDetailFeature.UpdateSeqRosterDetail;
using tas.Application.Service;

namespace tas.WebAPI.Controllers.Tas
{


    [Route("api/tas/[controller]")]
    [ApiController]
    [RoleAuthorize]
    public class RosterDetailController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<RosterDetailController> _logger;

        public RosterDetailController(IMediator mediator, ILogger<RosterDetailController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        [HttpGet("{RosterId}")]
        public async Task<ActionResult<List<GetRosterDetailResponse>>> GetAll(int RosterId, CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(new GetRosterDetailRequest(RosterId), cancellationToken);
            return Ok(response);
        }

        [HttpPost]
        public async Task<ActionResult> Create(CreateRosterDetailRequest request,
            CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(request, cancellationToken);
            return Ok(response);
        }


        [HttpPut]
        public async Task<ActionResult> Update(UpdateRosterDetailRequest request,
            CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(request, cancellationToken);
            return Ok(response);
        }

        [HttpDelete]
        public async Task<ActionResult> Delete(DeleteRosterDetailRequest request,
            CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(request, cancellationToken);
            return Ok(response);
        }



        [HttpPost("setseq")]
        public async Task<ActionResult> UpdateSeqNumber(UpdateSeqRosterDetailRequest request,
            CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(request, cancellationToken);
            return Ok(response);
        }


    }
}
