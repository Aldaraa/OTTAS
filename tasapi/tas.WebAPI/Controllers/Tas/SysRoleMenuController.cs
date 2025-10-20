using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using tas.Application.Features.CostCodeFeature.GetAllCostCode;
using tas.Application.Features.SysRoleMenuFeature.GetAllSysRoleMenu;
using tas.Application.Features.SysRoleMenuFeature.UpdateSysRoleMenu;
using tas.Application.Service;

namespace tas.WebAPI.Controllers.Tas
{


    [Route("api/tas/[controller]")]
    [ApiController]
    [RoleAuthorize]
    public class SysRoleMenuController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<SysRoleMenuController> _logger;

        public SysRoleMenuController(IMediator mediator, ILogger<SysRoleMenuController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        [HttpGet("getmenu/{roleId}")]
        public async Task<ActionResult<List<GetAllSysRoleMenuResponse>>> GetRoleMenu(int roleId, CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(new GetAllSysRoleMenuRequest(roleId), cancellationToken);
            return Ok(response);
        }

        [HttpPost]
        public async Task<ActionResult> UpdateRoleMenuPermission(UpdateSysRoleMenuRequest request, CancellationToken cancellationToken)
        {

           
            var response = await _mediator.Send(request, cancellationToken);
            return Ok(response);
        }
    }

}
