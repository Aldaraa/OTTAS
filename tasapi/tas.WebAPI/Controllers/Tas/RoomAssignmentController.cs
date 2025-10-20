using DocumentFormat.OpenXml.Office2010.Excel;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using tas.Application.Features.PositionFeature.GetAllRoomType;
using tas.Application.Features.RoomAssignmentFeature.FindAvailableByDatesAssignment;
using tas.Application.Features.RoomFeature.CreateRoomAssignment;
using tas.Application.Features.RoomFeature.CreateRoomAssignmentOwnership;
using tas.Application.Features.RoomFeature.GetAllActiveRoomAssignment;
using tas.Application.Features.RoomFeature.RemoveRoomAssignmentOwnership;
using tas.Application.Features.RoomFeature.RoomAssignmentEmployeeInfo;
using tas.Application.Features.RoomTypeFeature.CreateRoomType;
using tas.Application.Features.RoomTypeFeature.GetAllRoomType;
using tas.Application.Service;
using tas.Domain.Common;
using tas.Domain.Entities;

namespace tas.WebAPI.Controllers.Tas
{

    [Route("api/tas/[controller]")]
    [ApiController]
    [RoleAuthorize]


    public class RoomAssignmentController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<RoomAssignmentController> _logger;
        private readonly IMemoryCache _memoryCache;

        public RoomAssignmentController(IMediator mediator, ILogger<RoomAssignmentController> logger, IMemoryCache memoryCache)
        {
            _mediator = mediator;
            _logger = logger;
            _memoryCache = memoryCache;
        }

        [HttpGet("activerooms")]
        public async Task<ActionResult<List<GetAllActiveRoomAssignmentResponse>>> GetAllActiveRoomAssignment(
CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(new GetAllActiveRoomAssignmentRequest(), cancellationToken);
            return Ok(response);
        }

        [HttpGet("employeeinfo/{EmployeeId}")]
        public async Task<ActionResult<RoomAssignmentEmployeeInfoResponse>> EmployeeInfo(int EmployeeId, CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(new RoomAssignmentEmployeeInfoRequest(EmployeeId), cancellationToken);
            return Ok(response);
        }

        // FindAvailableByDatesAssignmentRequest


        //    [HttpGet("activerooms")]
        //    public async Task<ActionResult<List<GetAllActiveRoomAssignmentResponse>>> GetAllActiveRoomAssignment(
        //CancellationToken cancellationToken)
        //    {
        //        var response = await _mediator.Send(new GetAllActiveRoomAssignmentRequest(), cancellationToken);
        //        return Ok(response);
        //    }
        //findavailablebydatesassignment
        [HttpPost("FindAvailableByDatesAssignment")]
        public async Task<ActionResult> FindAvailableByDatesAssignment(FindAvailableByDatesAssignmentRequest request,
    CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(request, cancellationToken);
            return Ok(response);
        }


        [HttpPost("temporary")]
        public async Task<ActionResult> Create(CreateRoomAssignmentRequest request,
            CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(request, cancellationToken);
            return Ok(response);
        }



        [HttpPost("ownership")]
        public async Task<ActionResult> CreateOwnership(CreateRoomAssignmentOwnershipRequest request,
            CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(request, cancellationToken);
            string cacheEntityName = $"Employee_{request.EmployeeId}";
           _memoryCache.Remove($"API::{cacheEntityName}");
            return Ok(response);
        }




        [HttpPost("removeownership")]
        public async Task<ActionResult> RemoveOwnership(RemoveRoomAssignmentOwnershipRequest request,
            CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(request, cancellationToken);
            string cacheEntityName = $"Employee_{request.EmployeeId}";
            _memoryCache.Remove($"API::{cacheEntityName}");
            return Ok(response);
        }




    }


}
