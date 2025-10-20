using MediatR;
using Microsoft.AspNetCore.Mvc;
using tas.Application.Features.SysTeamFeature.GetSysTeam;
using tas.Application.Features.SysTeamFeature.GetAllSysTeam;
using tas.Application.Service;
using tas.Application.Features.SysTeamFeature.SetMenuSysTeam;
using tas.Application.Features.SysTeamFeature.SetUserSysTeam;
using tas.Application.Features.SysTeamFeature.DeleteUserSysTeam;
using Microsoft.AspNetCore.Authorization;

namespace tas.WebAPI.Controllers.Tas
{
    [Route("api/tas/[controller]")]
    [ApiController]
    [RoleAuthorize]
    public class SysTeamController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<SysTeamController> _logger;

        public SysTeamController(IMediator mediator, ILogger<SysTeamController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<List<GetAllSysTeamResponse>>> GetAll(CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(new GetAllSysTeamRequest(), cancellationToken);
            return Ok(response);
        }

        [HttpGet("{Id}")]
        public async Task<ActionResult<GetSysTeamResponse>> Get(int Id, CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(new GetSysTeamRequest(Id), cancellationToken);
            return Ok(response);
        }

        [HttpPost("setmenu")]
        public async Task<ActionResult> SetMenu(SetMenuSysTeamBulkRequest requests, CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(requests, cancellationToken);
            return Ok(response);
        }

        [HttpPost("setuser")]
        public async Task<ActionResult> SetUser(SetUserSysTeamRequest requests, CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(requests, cancellationToken);
            return Ok(response);
        }

        [HttpDelete("removeuser")]
      
        public async Task<ActionResult> RemoveUser(DeleteUserSysTeamRequest requests, CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(requests, cancellationToken);
            return Ok(response);
        }
    }
   
}
