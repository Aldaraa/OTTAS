using MediatR;
using Microsoft.AspNetCore.Mvc;
using tas.Application.Features.SysReportTemplateFeature.GetAllSysReportTemplate;
using tas.Application.Features.SysRoleEmployeeReportTemplateFeature.GetAllSysRoleEmployeeReportTemplate;
using tas.Application.Features.SysRoleEmployeeReportTemplateFeature.UpdateSysRoleEmployeeReportTemplate;
using tas.Application.Features.SysRoleReportTemplateFeature.GetAllSysRoleReportTemplate;
using tas.Application.Features.SysRoleReportTemplateFeature.UpdateSysRoleReportTemplate;
using tas.Application.Service;

namespace tas.WebAPI.Controllers.Tas
{


    [Route("api/tas/[controller]")]
    [ApiController]
    [RoleAuthorize]

    public class SysRoleEmployeeReportTemplateController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<SysRoleEmployeeReportTemplateController> _logger;

        public SysRoleEmployeeReportTemplateController(IMediator mediator, ILogger<SysRoleEmployeeReportTemplateController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }


        [HttpGet("{EmployeeId}")]
        public async Task<ActionResult<List<GetAllSysRoleEmployeeReportTemplateResponse>>> GetRoleReportTemplate(int EmployeeId, CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(new GetAllSysRoleEmployeeReportTemplateRequest(EmployeeId), cancellationToken);
            return Ok(response);
        }


        [HttpPost]
        public async Task<ActionResult> UpdateRoleReportTemplatePermission(UpdateSysRoleEmployeeReportTemplateRequest request, CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(request, cancellationToken);
            return Ok(response);
        }


    }
}
