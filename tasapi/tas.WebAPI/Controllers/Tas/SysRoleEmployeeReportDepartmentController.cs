using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using tas.Application.Features.SysRoleEmployeeReportDepartmentFeature.AddSysRoleEmployeeReportDepartment;
using tas.Application.Features.SysRoleEmployeeReportDepartmentFeature.DeleteSysRoleEmployeeReportDepartment;
using tas.Application.Features.SysRoleEmployeeReportDepartmentFeature.GetSysRoleEmployeeReportDepartment;
using tas.Application.Features.SysRoleEmployeeReportTemplateFeature.GetAllSysRoleEmployeeReportTemplate;
using tas.Application.Features.SysRoleEmployeeReportTemplateFeature.UpdateSysRoleEmployeeReportTemplate;
using tas.Application.Service;

namespace tas.WebAPI.Controllers.Tas
{

    [Route("api/tas/[controller]")]
    [ApiController]
    [RoleAuthorize]

    public class SysRoleEmployeeReportDepartmentController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<SysRoleEmployeeReportDepartmentController> _logger;

        public SysRoleEmployeeReportDepartmentController(IMediator mediator, ILogger<SysRoleEmployeeReportDepartmentController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }


        [HttpGet("{EmployeeId}")]
        public async Task<ActionResult<List<GetSysRoleEmployeeReportDepartmentResponse>>> GetData(int EmployeeId, CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(new GetSysRoleEmployeeReportDepartmentRequest(EmployeeId), cancellationToken);
            return Ok(response);
        }


        [HttpPost]
        public async Task<ActionResult> AddSysRoleEmployeeReportDepartment(AddSysRoleEmployeeReportDepartmentRequest request, CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(request, cancellationToken);
            return Ok(response);
        }


        [HttpDelete("{Id}")]
        public async Task<ActionResult> AddSysRoleEmployeeReportDepartment(int Id,CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(new DeleteSysRoleEmployeeReportDepartmentRequest(Id), cancellationToken);
            return Ok(response);
        }


    }
}
