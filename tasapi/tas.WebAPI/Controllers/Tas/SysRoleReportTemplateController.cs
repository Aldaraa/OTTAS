using MediatR;
using Microsoft.AspNetCore.Mvc;
using tas.Application.Features.CostCodeFeature.GetAllCostCode;
using tas.Application.Features.SysReportTemplateFeature.GetAllSysReportTemplate;
using tas.Application.Features.SysRoleMenuFeature.GetAllSysRoleMenu;
using tas.Application.Features.SysRoleMenuFeature.UpdateSysRoleMenu;
using tas.Application.Features.SysRoleReportTemplateFeature.GetAllSysRoleReportTemplate;
using tas.Application.Features.SysRoleReportTemplateFeature.UpdateSysRoleReportTemplate;
using tas.Application.Service;

namespace tas.WebAPI.Controllers.Tas
{

    [Route("api/tas/[controller]")]
    [ApiController]
    [RoleAuthorize]

    public class SysRoleReportTemplateController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<SysRoleMenuController> _logger;

        public SysRoleReportTemplateController(IMediator mediator, ILogger<SysRoleMenuController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }


        [HttpGet("{roleId}")]
        public async Task<ActionResult<List<GetAllSysRoleReportTemplateResponse>>> GetRoleReportTemplate(int roleId, CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(new GetAllSysRoleReportTemplateRequest(roleId), cancellationToken);
            return Ok(response);
        }


        [HttpPost]
        public async Task<ActionResult> UpdateRoleReportTemplatePermission(UpdateSysRoleReportTemplateRequest request, CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(request, cancellationToken);
            return Ok(response);
        }


    }

}
