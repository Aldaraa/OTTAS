using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using tas.Application.Features.SysRoleEmployeeReportEmployerFeature.AddSysRoleEmployeeReportEmployer;
using tas.Application.Features.SysRoleEmployeeReportEmployerFeature.DeleteSysRoleEmployeeReportEmployer;
using tas.Application.Features.SysRoleEmployeeReportEmployerFeature.GetSysRoleEmployeeReportEmployer;
using tas.Application.Features.SysRoleEmployeeReportTemplateFeature.GetAllSysRoleEmployeeReportTemplate;
using tas.Application.Features.SysRoleEmployeeReportTemplateFeature.UpdateSysRoleEmployeeReportTemplate;
using tas.Application.Service;

namespace tas.WebAPI.Controllers.Tas
{

    [Route("api/tas/[controller]")]
    [ApiController]
    [RoleAuthorize]

    public class SysRoleEmployeeReportEmployerController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<SysRoleEmployeeReportEmployerController> _logger;

        public SysRoleEmployeeReportEmployerController(IMediator mediator, ILogger<SysRoleEmployeeReportEmployerController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }


        [HttpGet("{EmployeeId}")]
        public async Task<ActionResult<List<GetSysRoleEmployeeReportEmployerResponse>>> GetData(int EmployeeId, CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(new GetSysRoleEmployeeReportEmployerRequest(EmployeeId), cancellationToken);
            return Ok(response);
        }


        [HttpPost]
        public async Task<ActionResult> AddSysRoleEmployeeReportEmployer(AddSysRoleEmployeeReportEmployerRequest request, CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(request, cancellationToken);
            return Ok(response);
        }


        [HttpDelete("{Id}")]
        public async Task<ActionResult> AddSysRoleEmployeeReportEmployer(int Id, CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(new DeleteSysRoleEmployeeReportEmployerRequest(Id), cancellationToken);
            return Ok(response);
        }


    }
}
