using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using tas.Application.Features.DepartmentFeature.CreateDepartment;
using tas.Application.Features.DepartmentFeature.GetAllDepartment;
using tas.Application.Features.GroupDetailFeature.CreateGroupDetail;
using tas.Application.Features.GroupMembersFeature.CreateGroupMembers;
using tas.Application.Service;
using tas.Domain.Entities;

namespace tas.WebAPI.Controllers.Tas
{
    [Route("api/tas/[controller]")]
    [ApiController]
    [RoleAuthorize]
    public class GroupMembersController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<GroupMembersController> _logger;

        public GroupMembersController(IMediator mediator, ILogger<GroupMembersController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }


        [HttpPost("{EmployeeId}")]
        public async Task<ActionResult> Create(int EmployeeId, List<GroupMembersGroup> request,
            CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(new CreateGroupMembersRequest(EmployeeId, request), cancellationToken);
            return Ok(response);
        }

    }

}
