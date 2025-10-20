using Application.Features.HotDeskFeature.DepartmentSend;
using Application.Features.HotDeskFeature.EmployeeInfo;
using Application.Features.HotDeskFeature.EmployeeInfoById;
using Application.Features.HotDeskFeature.EmployeeSend;
using Application.Features.HotDeskFeature.EmployeeStatusInfoById;
using Application.Features.HotDeskFeature.EmployeeStatusSend;
using Application.Repositories;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace tas.IntegrationAPI.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class HotDeskController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<HotDeskController> _logger;
        private readonly IMediator _mediator;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMemoryCache _memoryCache;
        private readonly ITransportInfoRepository _transportInfoRepository;
        public HotDeskController(IMediator mediator, IConfiguration configuration, ILogger<HotDeskController> logger, IHttpContextAccessor httpContextAccessor, IMemoryCache memoryCache, ITransportInfoRepository transportInfoRepository)
        {
            _configuration = configuration;
            _logger = logger;
            _mediator = mediator;
            _httpContextAccessor = httpContextAccessor;
            _memoryCache = memoryCache;
            _transportInfoRepository = transportInfoRepository;
        }



        /// <summary>
        /// Gets transport information for an employee.
        /// </summary>
        /// <param name="request">The transport info request containing SAPID.</param>
        /// <returns>A list of transport information responses.</returns>
        /// <response code="200">Returns the list of transport information.</response>
        /// <response code="400">If the SAPID is null or zero.</response>
        [HttpPost("employeelist")]
        public async Task<ActionResult> EmployeeInfo(EmployeeInfoRequest request, CancellationToken cancellationToken)
        {
            try
            {
               var data =  await _mediator.Send(request, cancellationToken);
                return Ok(data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return BadRequest(ex.Message);
            }
        }



        [HttpGet("employeeinfo/{employeeId}")]
        public async Task<ActionResult> EmployeeInfoById(int employeeId, CancellationToken cancellationToken)
        {
            try
            {
                var data = await _mediator.Send(new EmployeeInfoByIdRequest(employeeId), cancellationToken);
                return Ok(data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return BadRequest(ex.Message);
            }
        }
        





        [HttpGet("employeesend")]
        public async Task<ActionResult> SendEmployeeInfo(CancellationToken cancellationToken)
        {
            try
            {
                await _mediator.Send(new EmployeeSendRequest(), cancellationToken);
                return Ok("OK");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return BadRequest(ex.Message);
            }
        }


        [HttpGet("employeestatussend")]
        public async Task<ActionResult> SendEmployeeStatusInfo(CancellationToken cancellationToken)
        {
            try
            {
                await _mediator.Send(new EmployeeStatusSendRequest(), cancellationToken);
                return Ok("OK");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("employeestatus/{employeeId}")]
        public async Task<ActionResult> EmployeeStatusInfoById(int employeeId, CancellationToken cancellationToken)
        {
            try
            {
               var data = await _mediator.Send(new EmployeeStatusInfoByIdRequest(employeeId), cancellationToken);
                return Ok(data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return BadRequest(ex.Message);
            }
        }



        [HttpGet("departmentsend")]
        public async Task<ActionResult> SendDepartmentInfo(CancellationToken cancellationToken)
        {
            try
            {
                await _mediator.Send(new DepartmentSendRequest(), cancellationToken);
                return Ok("OK");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return BadRequest(ex.Message);
            }
        }

    }
}
