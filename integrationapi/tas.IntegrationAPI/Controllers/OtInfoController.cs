using Application.Features.OtinfoFeature.CheckTransport;
using Application.Features.OtinfoFeature.JobInfo;
using Application.Features.OtinfoFeature.ManualSent;
using Application.Features.TransportFeature.TransportInfo;
using Application.Repositories;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Swashbuckle.AspNetCore.Annotations;

namespace tas.IntegrationAPI.Controllers
{


    [Route("api/[controller]")]
    [ApiController]
    public class OtInfoController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<OtInfoController> _logger;
        private readonly IMediator _mediator;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMemoryCache _memoryCache;
        private readonly ITransportInfoRepository _transportInfoRepository;
        public OtInfoController(IMediator mediator, IConfiguration configuration, ILogger<OtInfoController> logger, IHttpContextAccessor httpContextAccessor, IMemoryCache memoryCache, ITransportInfoRepository transportInfoRepository)
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
        [HttpPost("manualsent")]
        public async Task<ActionResult> ManualSentOtinfo(ManualSentRequest request, CancellationToken cancellationToken)
        {
            try
            {
                await _mediator.Send(request, cancellationToken);
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return BadRequest(ex.Message);
            }
        }




        /// <summary>
        /// Gets transport information for an employee.
        /// </summary>
        /// <param name="request">The transport info request containing SAPID.</param>
        /// <returns>A list of transport information responses.</returns>
        /// <response code="200">Returns the list of transport information.</response>
        /// <response code="400">If the SAPID is null or zero.</response>
        [HttpPost("checktransport")]
        public async Task<ActionResult> Checktransport(CheckTransportRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var transportResponse = await _mediator.Send(request, cancellationToken);
                return Ok(transportResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return BadRequest(ex.Message);
            }
        }



        /// <summary>
        /// Gets transport information for an employee.
        /// </summary>
        /// <returns>A list of transport information responses.</returns>
        /// <response code="200">Returns the list of transport information.</response>
        /// <response code="400">If the SAPID is null or zero.</response>
        [HttpGet("jobinfo")]
        public async Task<ActionResult> JobInfo(CancellationToken cancellationToken)
        {
            try
            {
                var transportResponse = await _mediator.Send(new JobInfoRequest(), cancellationToken);
                return Ok(transportResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return BadRequest(ex.Message);
            }
        }

    }
}
