using MediatR;
using Microsoft.AspNetCore.Mvc;
using tas.Application.Features.ColorFeature.CreateColor;
using tas.Application.Features.ColorFeature.DeleteColor;
using tas.Application.Features.ColorFeature.GetAllColor;
using tas.Application.Features.DepartmentGroupConfigFeature.CreateDepartmentGroupConfig;
using tas.Application.Features.DepartmentGroupConfigFeature.DeleteDepartmentGroupConfig;
using tas.Application.Features.DepartmentGroupConfigFeature.GetDepartmentGroupConfig;
using tas.Application.Service;

namespace tas.WebAPI.Controllers.Tas
{

    [Route("api/tas/[controller]")]
    [ApiController]
    [RoleAuthorize]

    public class DepartmentGroupConfigController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<DepartmentGroupConfigController> _logger;

        public DepartmentGroupConfigController(IMediator mediator, ILogger<DepartmentGroupConfigController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }




        [HttpGet("{DepartmentId}")]
        public async Task<ActionResult<List<GetAllColorResponse>>> GetDepartmentGroupConfig(int DepartmentId, CancellationToken cancellationToken)
        {

            var response = await _mediator.Send(new GetDepartmentGroupConfigRequest(DepartmentId), cancellationToken);
            return Ok(response);


        }


        [HttpPost]
        public async Task<ActionResult> Create(CreateDepartmentGroupConfigRequest request,
            CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(request, cancellationToken);
            return Ok(response);
        }


        [HttpDelete]
        public async Task<ActionResult> Delete(DeleteDepartmentGroupConfigRequest request,
    CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(request, cancellationToken);
            return Ok(response);
        }




    }

}
