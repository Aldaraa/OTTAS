using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using tas.Application.Features.FlightGroupDetailFeature.SetClusterFlightGroupDetail;
using tas.Application.Features.FlightGroupDetailFeature.UpdateShiftFlightGroupDetail;
using tas.Application.Features.FlightGroupMasterFeature.CreateFlightGroupMaster;
using tas.Application.Features.FlightGroupMasterFeature.GetAllFlightGroupMaster;
using tas.Application.Features.FlightGroupMasterFeature.GetFlightGroupMaster;
using tas.Application.Service;
using tas.Domain.Entities;

namespace tas.WebAPI.Controllers.Tas
{
    [Route("api/tas/[controller]")]
    [ApiController]
    [RoleAuthorize]
    public class FlightGroupDetailController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<FlightGroupDetailController> _logger;
        private readonly CacheService _memoryCache;

        public FlightGroupDetailController(IMediator mediator, ILogger<FlightGroupDetailController> logger, CacheService memoryCache)
        {
            _mediator = mediator;
            _logger = logger;
            _memoryCache = memoryCache;
        }


        [HttpPost("setcluster")]
        public async Task<ActionResult> SetCluster(SetClusterFlightGroupDetailRequest request,
            CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(request, cancellationToken);
            _memoryCache.Remove($"API::{typeof(FlightGroupMaster).Name}_1");
            _memoryCache.Remove($"API::{typeof(FlightGroupMaster).Name}_0");
            _memoryCache.Remove($"API::{typeof(FlightGroupMaster).Name}");
            return Ok(response);
        }


        [HttpPut("shift")]
        public async Task<ActionResult> UpdateShift(UpdateShiftFlightGroupDetailRequest request,
            CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(request, cancellationToken);
            _memoryCache.RemoveByPrefix($"API::{typeof(FlightGroupMaster).Name}");
            return Ok(response);
        }


        
    }

}
