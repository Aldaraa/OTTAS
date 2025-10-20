using MediatR;
using Microsoft.AspNetCore.Mvc;
using tas.Application.Features.SysRoleEmployeeDashboardFeature.GetAllSysRoleEmployeeDashboard;
using tas.Application.Features.SysRoleEmployeeDashboardFeature.UpdateSysRoleEmployeeDashboard;
using tas.Application.Service;

namespace tas.WebAPI.Controllers.Tas
{
    [Route("api/tas/[controller]")]
    [ApiController]
    [RoleAuthorize]
    public class SysRoleEmployeeDashboardController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<SysRoleEmployeeDashboardController> _logger;

        public SysRoleEmployeeDashboardController(IMediator mediator, ILogger<SysRoleEmployeeDashboardController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }


        [HttpGet("getDashboard/{EmployeeId}")]
        public async Task<ActionResult<List<GetAllSysRoleEmployeeDashboardResponse>>> GetSysRoleEmployeeDashboard(int EmployeeId, CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(new GetAllSysRoleEmployeeDashboardRequest(EmployeeId), cancellationToken);
            return Ok(response);
        }


        [HttpPost]
        public async Task<ActionResult> UpdateSysRoleEmployeeDashboardPermission(UpdateSysRoleEmployeeDashboardRequest request, CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(request, cancellationToken);
            return Ok(response);
        }

    }
}
