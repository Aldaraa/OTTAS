using DocumentFormat.OpenXml.InkML;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Caching.Memory;
using tas.Application.Features.BedFeature.GetBed;
using tas.Application.Features.SysRoleFeature.AddEmployee;
using tas.Application.Features.SysRoleFeature.GetAllSysRole;
using tas.Application.Features.SysRoleFeature.GetEmployeeRoleInfo;
using tas.Application.Features.SysRoleFeature.GetSysRole;
using tas.Application.Features.SysRoleFeature.RemoveEmployeeRole;
using tas.Application.Features.SysRoleFeature.UpdateReadOnlyAccesss;
using tas.Application.Service;

namespace tas.WebAPI.Controllers.Tas
{

    [Route("api/tas/[controller]")]
    [ApiController]
    [RoleAuthorize]
    public class SysRoleController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<SysRoleController> _logger;
        private readonly IMemoryCache _memoryCache;
        private readonly SignalrHub _signalrhub;
        private readonly IHubContext<SignalrHub> _hubContext;

        public SysRoleController(IMediator mediator, ILogger<SysRoleController> logger, IMemoryCache memoryCache, IHubContext<SignalrHub> hubContext, SignalrHub signalrHub)
        {
            _mediator = mediator;
            _logger = logger;
            _memoryCache = memoryCache;
            _hubContext = hubContext;
            _signalrhub = signalrHub;   
        }


        [HttpGet]
        public async Task<ActionResult<List<GetAllSysRoleResponse>>> Get(CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(new GetAllSysRoleRequest(), cancellationToken); ;
            return Ok(response);
        }

        [HttpGet("{RoleId}")]
        public async Task<ActionResult<List<GetSysRoleResponse>>> Get(int RoleId, CancellationToken  cancellationToken)
        {
            var response = await _mediator.Send(new GetSysRoleRequest(RoleId), cancellationToken); ;
            return Ok(response);
        }

        [HttpPost("adduser")]
        public async Task<ActionResult> AddEmployee(AddEmployeeRequest request, CancellationToken cancellationToken)
        {
           
                var response = await _mediator.Send(request, cancellationToken);
            return Ok(response);
        }


        [HttpPut("accesspermission")]
        public async Task<ActionResult> AddEmployee(UpdateReadOnlyAccesssRequest request, CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(request, cancellationToken);
            

            return Ok(response);
        }


        [HttpDelete("removeuser/{id}")]
        public async Task<ActionResult> RemoveEmployee(int id, CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(new RemoveEmployeeRoleRequest(id), cancellationToken);
            
            return Ok(response);
        }


        [HttpGet("roleinfo/{EmployeeId}")]
        public async Task<ActionResult> EmployeeRoleInfo(int EmployeeId, CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(new GetEmployeeRoleInfoRequest(EmployeeId), cancellationToken);

            return Ok(response);
        }

        //427329

        [HttpGet("testsignal")]
        public async Task<ActionResult> TestSignal(int EmployeeId, CancellationToken cancellationToken)
        {
            //   await     _hubContext.RoleChange(Convert.ToString(EmployeeId));
            await _signalrhub.RoleChange(Convert.ToString(EmployeeId));
            return Ok("Sigal sended");
        }
    }

}