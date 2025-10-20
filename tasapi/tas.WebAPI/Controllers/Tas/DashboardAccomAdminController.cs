using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using tas.Application.Features.DashboardAccomAdminFeature.GetCampInfo;
using tas.Application.Features.DashboardAccomAdminFeature.GetCampUsageInfo;
using tas.Application.Features.DashboardAccomAdminFeature.GetNonSiteInfo;
using tas.Application.Features.DashboardAccomAdminFeature.GetNoRoomInfo;
using tas.Application.Features.DashboardAccomAdminFeature.GetOccupantsInfo;
using tas.Application.Features.DashboardAccomAdminFeature.GetPobInfo;
using tas.Application.Features.DashboardDataAdminFeature.GetPackMealData;
using tas.Application.Service;
using tas.Domain.Common;

namespace tas.WebAPI.Controllers.Tas
{

    [Route("api/tas/[controller]")]
    [ApiController]
    [RoleAuthorize]
    public class DashboardAccomAdminController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<DashboardAccomAdminController> _logger;
        private readonly CacheService _memoryCache;


        public DashboardAccomAdminController(IMediator mediator, ILogger<DashboardAccomAdminController> logger, CacheService memoryCache)
        {
            _mediator = mediator;
            _logger = logger;
            _memoryCache = memoryCache;
        }

        [HttpGet("campinfo")]
        public async Task<ActionResult<GetCampInfoResponse>> GetCampInfo(CancellationToken cancellationToken)
        {

            var outData = new GetCampInfoResponse();
            string cacheEntityName = $"DashboardAccomAdmin_campinfo";

            if (_memoryCache.TryGetValue($"API::{cacheEntityName}", out outData))
            {
                return Ok(outData);
            }
            else
            {
                var response = await _mediator.Send(new GetCampInfoRequest(), cancellationToken);
                _memoryCache.Set($"API::{cacheEntityName}", response, TimeSpan.FromMinutes(GlobalConstants.ENDPOINT_TAS_DASHBOARD_CACHE_MINUTE));
                return Ok(response);

            }


        }


        [HttpGet("usageinfo")]
        public async Task<ActionResult<GetCampUsageInfoResponse>> GetCampUsage(DateTime? currentDate, CancellationToken cancellationToken)
        {

            var outData = new GetCampUsageInfoResponse();
            string cacheEntityName = $"DashboardAccomAdmin_campusageinfo_{currentDate?.ToString("yyyy-MM-dd") ?? "null"}";

            if (_memoryCache.TryGetValue($"API::{cacheEntityName}", out outData))
            {
                return Ok(outData);
            }
            else
            {
                var response = await _mediator.Send(new GetCampUsageInfoRequest(currentDate), cancellationToken);
                _memoryCache.Set($"API::{cacheEntityName}", response, TimeSpan.FromMinutes(GlobalConstants.ENDPOINT_TAS_DASHBOARD_CACHE_MINUTE));
                return Ok(response);

            }


        }



        [HttpGet("nonsiteinfo")]
        public async Task<ActionResult<GetNonSiteInfoResponse>> GetNonSiteInfo(DateTime? currentDate, CancellationToken cancellationToken)
        {
            var outData = new GetNonSiteInfoResponse();
            string cacheEntityName = $"DashboardAccomAdmin_nonsiteinfo_{currentDate?.ToString("yyyy-MM-dd") ?? "null"}";

            if (_memoryCache.TryGetValue($"API::{cacheEntityName}", out outData))
            {
                return Ok(outData);
            }
            else
            {
                var response = await _mediator.Send(new GetNonSiteInfoRequest(currentDate), cancellationToken);
                _memoryCache.Set($"API::{cacheEntityName}", response, TimeSpan.FromMinutes(GlobalConstants.ENDPOINT_TAS_DASHBOARD_CACHE_MINUTE));
                return Ok(response);
            }


        }



        [HttpGet("noroom")]
        public async Task<ActionResult<GetNoRoomInfoResponse>> GetNoRoomInfo(DateTime? currentDate, CancellationToken cancellationToken)
        {
            var outData = new GetNoRoomInfoResponse();
            string cacheEntityName = $"DashboardAccomAdmin_noroominfo_{currentDate?.ToString("yyyy-MM-dd") ?? "null"}";

            if (_memoryCache.TryGetValue($"API::{cacheEntityName}", out outData))
            {
                return Ok(outData);
            }
            else
            {
                var response = await _mediator.Send(new GetNoRoomInfoRequest(currentDate), cancellationToken);
                _memoryCache.Set($"API::{cacheEntityName}", response, TimeSpan.FromMinutes(GlobalConstants.ENDPOINT_TAS_DASHBOARD_CACHE_MINUTE));
                return Ok(response);
            }



        }



        [HttpGet("pob")]
        public async Task<ActionResult<GetPobInfoResponse>> GetPobInfo(DateTime? currentDate, CancellationToken cancellationToken)
        {
            var outData = new GetPobInfoResponse();
            string cacheEntityName = $"DashboardAccomAdmin_pobinfo_{currentDate?.ToString("yyyy-MM-dd") ?? "null"}";

            if (_memoryCache.TryGetValue($"API::{cacheEntityName}", out outData))
            {
                return Ok(outData);
            }
            else
            {
                var response = await _mediator.Send(new GetPobInfoRequest(currentDate), cancellationToken);
                _memoryCache.Set($"API::{cacheEntityName}", response, TimeSpan.FromMinutes(GlobalConstants.ENDPOINT_TAS_DASHBOARD_CACHE_MINUTE));
                return Ok(response);
            }


        }

        [HttpGet("occupants")]
        public async Task<ActionResult<GetOccupantsInfoResponse>> GetOccupants(DateTime? currentDate, CancellationToken cancellationToken)
        {
            var outData = new GetOccupantsInfoResponse();
            string cacheEntityName = $"DashboardAccomAdmin_occupantsinfo_{currentDate?.ToString("yyyy-MM-dd") ?? "null"}";

            if (_memoryCache.TryGetValue($"API::{cacheEntityName}", out outData))
            {
                return Ok(outData);
            }
            else
            {
                var response = await _mediator.Send(new GetOccupantsInfoRequest(currentDate), cancellationToken);
                _memoryCache.Set($"API::{cacheEntityName}", response, TimeSpan.FromMinutes(GlobalConstants.ENDPOINT_TAS_DASHBOARD_CACHE_MINUTE));
                return Ok(response);
            }


        }

    }

}
