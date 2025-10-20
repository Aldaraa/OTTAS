using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using tas.Application.Common.Exceptions;
using tas.Application.Features.AuditFeature.GetEmployeeAudit;
using tas.Application.Features.AuditFeature.GetGroupMembersAudit;
using tas.Application.Features.AuditFeature.GetMasterAudit;
using tas.Application.Features.AuditFeature.GetRoomAudit;
using tas.Application.Features.AuditFeature.GetRoomAuditByRoom;
using tas.Application.Features.AuditFeature.GetTransportAudit;
using tas.Application.Features.BedFeature.GetBed;
using tas.Application.Service;
using tas.Domain.Enums;

namespace tas.WebAPI.Controllers.Tas
{
    [Route("api/tas/[controller]")]
    [ApiController]
    [RoleAuthorize]
    public class AuditController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<AuditController> _logger;

        public AuditController(IMediator mediator, ILogger<AuditController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }



        [HttpGet("profileaudit/{Id}")]
        public async Task<ActionResult> GetRoomData(int Id, DateTime? startDate, DateTime? endDate, CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(new GetEmployeeAuditRequest(Id, startDate, endDate) , cancellationToken);
            return Ok(response);
        }


        [HttpPost("roomauditbyroom")]
        public async Task<ActionResult> GetRoomByRoomAuditData(GetRoomAuditByRoomRequest request, CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(request, cancellationToken);
            if (response.ExcelFile != null)
            {
              //  return File(response.ExcelFile, "application/force-download", $"TAS_RoomAuditByRoom_Download_{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}.xlsx");

                    var excelFile = response.ExcelFile;
                    Response.Headers.Add("Access-Control-Expose-Headers", "Content-Disposition");
                    Response.Headers.Add("Content-Disposition", $"attachment; filename=TAS_RoomAuditByRoom_Download_{ DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}.xlsx");
                    return File(excelFile.ToArray(), "application/xlsx");
               

            }
            else
            {
                return NoContent();
            }
        }


        [HttpPost("roomaudit")]
        public async Task<ActionResult> GetRoomData(GetRoomAuditRequest request, CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(request, cancellationToken);
            if (response.ExcelFile != null)
            {
                return File(response.ExcelFile, "application/force-download", $"TAS_RoomAudit_Download_{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}.xlsx"); ;
            }
            else {
                return NoContent();
            }
        }

        [HttpPost("transportaudit")]
        public async Task<ActionResult<GetBedResponse>> GetTransportData(GetTransportAuditRequest request, CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(request, cancellationToken);
            if (response.ExcelFile != null)
            {
                return File(response.ExcelFile, "application/force-download", $"TAS_TransportAudit_Download_{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}.xlsx"); ;
            }
            else
            {
                return NoContent();
            }
        }


        [HttpGet("masteraudit/{tablename}")]
        public async Task<ActionResult> GetRoomData(string tablename, int? recordId, DateTime? startDate, DateTime? endDate, CancellationToken cancellationToken)
        {
            if (AuditConstants.MASTER_AUDIT_TABLES.Contains(tablename))
            {
                var response = await _mediator.Send(new GetMasterAuditRequest(tablename, recordId, startDate, endDate), cancellationToken);
                return Ok(response);
            }
            else
            {
                throw new BadRequestException("This table cannot be audited");
            }


        }

        [HttpGet("masteraudit/requiredtables")]
        public async Task<ActionResult> GetTables()
        {
            return Ok(AuditConstants.MASTER_AUDIT_TABLES);
        }


        [HttpGet("groupmembers/{employeeId}/{groupMasterId}")]
        public async Task<ActionResult> GetGroupMembersAudit(int employeeId, int groupMasterId, CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(new GetGroupMembersAuditRequest(employeeId, groupMasterId), cancellationToken);
            return Ok(response);

        }


    }
}




