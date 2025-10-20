using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using tas.Application.Features.RequestGroupEmployeeFeature.AddRequestGroupEmployees;
using tas.Application.Features.RequestGroupEmployeeFeature.GetAllRequestGroupEmployees;
using tas.Application.Features.RequestGroupEmployeeFeature.GetRequestGroupEmployees;
using tas.Application.Features.RequestGroupEmployeeFeature.GetRequestGroupInpersonateUsers;
using tas.Application.Features.RequestGroupEmployeeFeature.GetRequestLineManagerAdminEmployees;
using tas.Application.Features.RequestGroupEmployeeFeature.GetRequestLineManagerEmployees;
using tas.Application.Features.RequestGroupEmployeeFeature.OrderRequestGroupEmployees;
using tas.Application.Features.RequestGroupEmployeeFeature.RemoveRequestGroupEmployees;
using tas.Application.Features.RequestGroupEmployeeFeature.RequestGroupActiveEmployees;
using tas.Application.Features.RequestGroupEmployeeFeature.SetPrimaryRequestGroupEmployees;
using tas.Application.Features.RequestGroupEmployeeFeature.UpdateRequestGroupEmployees;
using tas.Application.Features.RequestGroupFeature.GetAllRequestGroup;
using tas.Application.Service;

namespace tas.WebAPI.Controllers.Tas
{

    [Route("api/tas/[controller]")]
    [ApiController]
    [RoleAuthorize]
    public class RequestGroupEmployeeController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<RequestGroupEmployeeController> _logger;

        public RequestGroupEmployeeController(IMediator mediator, ILogger<RequestGroupEmployeeController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<List<GetAllRequestGroupEmployeesResponse>>> GetAllEmployees(CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(new GetAllRequestGroupEmployeesRequest(), cancellationToken);
            return Ok(response);
        }
        
        
        [HttpGet("activeemployees")]
        public async Task<ActionResult<List<RequestGroupActiveEmployeesResponse>>> GetActiveEmployees(CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(new RequestGroupActiveEmployeesRequest(), cancellationToken);
            return Ok(response);
        }


        [HttpGet("inpersonateusers")]
        public async Task<ActionResult<List<GetRequestGroupInpersonateUsersResponse>>> GetInpersnateusers(CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(new GetRequestGroupInpersonateUsersRequest(), cancellationToken);
            return Ok(response);
        }

        [HttpGet("groupemployees/{groupId}")]
        public async Task<ActionResult<List<GetRequestGroupEmployeesResponse>>> GetGroupEmployees(int groupId, CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(new GetRequestGroupEmployeesRequest(groupId), cancellationToken);
            return Ok(response);
        }

        [HttpGet("linemanageremployees")]
        public async Task<ActionResult<List<GetRequestLineManagerEmployeesResponse>>> GetLineManagerEmployees(CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(new GetRequestLineManagerEmployeesRequest(), cancellationToken);
            return Ok(response);
        }

        [HttpGet("linemanageradminemployees")]
        public async Task<ActionResult<List<GetRequestLineManagerAdminEmployeesResponse>>> GetLineManagerAdminEmployees(CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(new GetRequestLineManagerAdminEmployeesRequest(), cancellationToken);
            return Ok(response);
        }


        [HttpPost("groupemployees")]
        public async Task<ActionResult> AddGroupEmployees(AddRequestGroupEmployeesRequest request, CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(request, cancellationToken);
            return Ok(response);
        }


        [HttpDelete("groupemployees")]
        public async Task<ActionResult> RemoveGroupEmployees(RemoveRequestGroupEmployeesRequest request, CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(request, cancellationToken);
            return Ok(response);
        }



        [HttpPut("groupemployees")]
        public async Task<ActionResult> UpdateGroupEmployees(UpdateRequestGroupEmployeesRequest request, CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(request, cancellationToken);
            return Ok(response);
        }

        [HttpPut("setprimary")]
        public async Task<ActionResult> SetPrimaryGroupEmployees(SetPrimaryRequestGroupEmployeesRequest request, CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(request, cancellationToken);
            return Ok(response);
        }

        [HttpPut("orderemployees")]
        public async Task<ActionResult> OrderGroupEmployees(OrderRequestGroupEmployeesRequest request, CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(request, cancellationToken);
            return Ok(response);
        }


    }
}

