using MediatR;
using Microsoft.AspNetCore.Mvc;
using tas.Application.Features.CostCodeFeature.GetAllCostCode;
using tas.Application.Features.SysRoleEmployeeMenuFeature.GetAllSysRoleEmployeeMenu;
using tas.Application.Features.SysRoleEmployeeMenuFeature.UpdateSysRoleEmployeeMenu;
using tas.Application.Service;

namespace tas.WebAPI.Controllers.Tas
{

    [Route("api/tas/[controller]")]
    [ApiController]
    [RoleAuthorize]
    public class SysRoleEmployeeMenuController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<SysRoleEmployeeMenuController> _logger;

        public SysRoleEmployeeMenuController(IMediator mediator, ILogger<SysRoleEmployeeMenuController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }


        [HttpGet("getmenu/{EmployeeId}")]
        public async Task<ActionResult<List<GetAllSysRoleEmployeeMenuResponse>>> GetSysRoleEmployeeMenu(int EmployeeId, CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(new GetAllSysRoleEmployeeMenuRequest(EmployeeId), cancellationToken);
            return Ok(response);
        }


        [HttpPost]
        public async Task<ActionResult> UpdateSysRoleEmployeeMenuPermission(UpdateSysRoleEmployeeMenuRequest request, CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(request, cancellationToken);
            return Ok(response);
        }

    }

}
