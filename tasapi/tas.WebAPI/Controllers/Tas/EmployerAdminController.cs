using MediatR;
using Microsoft.AspNetCore.Mvc;
using tas.Application.Features.EmployerAdminFeature.AddEmployerAdmin;
using tas.Application.Features.EmployerAdminFeature.DeleteEmployerAdmin;
using tas.Application.Features.EmployerAdminFeature.GetEmployerAdmin;
using tas.Application.Service;

namespace tas.WebAPI.Controllers.Tas
{

    [Route("api/tas/[controller]")]
    [ApiController]
    [RoleAuthorize]

    public class EmployerAdminController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<EmployerAdminController> _logger;

        public EmployerAdminController(IMediator mediator, ILogger<EmployerAdminController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }


        [HttpGet("{EmployeeId}")]
        public async Task<ActionResult<List<GetEmployerAdminResponse>>> GetData(int EmployeeId, CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(new GetEmployerAdminRequest(EmployeeId), cancellationToken);
            return Ok(response);
        }


        [HttpPost]
        public async Task<ActionResult> AddEmployerAdmin(AddEmployerAdminRequest request, CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(request, cancellationToken);
            return Ok(response);
        }


        [HttpDelete("{Id}")]
        public async Task<ActionResult> AddEmployerAdmin(int Id, CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(new DeleteEmployerAdminRequest(Id), cancellationToken);
            return Ok(response);
        }


    }


}
