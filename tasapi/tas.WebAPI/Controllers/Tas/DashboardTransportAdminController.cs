using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using tas.Application.Features.DashboardAccomAdminFeature.GetCampInfo;
using tas.Application.Features.DashboardAccomAdminFeature.GetOccupantsInfo;
using tas.Application.Features.DashboardTransportAdminFeature.GetDomesticData;
using tas.Application.Features.DashboardTransportAdminFeature.GetInternationalTravelData;
using tas.Application.Features.DashboardTransportAdminFeature.GetRosterData;
using tas.Application.Features.DashboardTransportAdminFeature.GetTransportGroupData;
using tas.Application.Features.DashboardTransportAdminFeature.GetTransportGroupEmployeeData;
using tas.Application.Service;
using tas.Domain.Common;

namespace tas.WebAPI.Controllers.Tas
{
    [Route("api/tas/[controller]")]
    [ApiController]
    [RoleAuthorize]
    public class DashboardTransportAdminController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<DashboardTransportAdminController> _logger;
        private readonly CacheService _memoryCache;


        public DashboardTransportAdminController(IMediator mediator, ILogger<DashboardTransportAdminController> logger, CacheService memoryCache)
        {
            _mediator = mediator;
            _logger = logger;
            _memoryCache = memoryCache;
        }

        [HttpGet("international")]
        public async Task<ActionResult<GetInternationalTravelDataResponse>> GetInternationData(DateTime? startDate, DateTime endDate, CancellationToken cancellationToken)
        {

            var outData = new GetCampInfoResponse();
            string cacheEntityName = $"DashboardTransportAdmin_international_{startDate?.ToString("yyyy-MM-dd") ?? "null"}_{endDate.ToString("yyyy-MM-dd") ?? "null"}";

            if (_memoryCache.TryGetValue($"API::{cacheEntityName}", out outData))
            {
                return Ok(outData);
            }
            else
            {
                var response = await _mediator.Send(new GetInternationalTravelDataRequest(startDate, endDate), cancellationToken);
                _memoryCache.Set($"API::{cacheEntityName}", response, TimeSpan.FromMinutes(GlobalConstants.ENDPOINT_TAS_DASHBOARD_CACHE_MINUTE));
                return Ok(response);

            }



        }


        [HttpGet("roster/week")]
        public async Task<ActionResult<GetRosterDataResponse>> GetRosterWeeklyData(DateTime? startDate, CancellationToken cancellationToken)
        {

            var outData = new GetCampInfoResponse();
            string cacheEntityName = $"DashboardTransportAdmin_roster_weekly_{startDate?.ToString("yyyy-MM-dd")}";

            if (_memoryCache.TryGetValue($"API::{cacheEntityName}", out outData))
            {
                return Ok(outData);
            }
            else
            {
                var response = await _mediator.Send(new GetRosterDataRequest(startDate, "WEEKLY"), cancellationToken);
                _memoryCache.Set($"API::{cacheEntityName}", response, TimeSpan.FromMinutes(GlobalConstants.ENDPOINT_TAS_DASHBOARD_CACHE_MINUTE));
                return Ok(response);

            }
        }


        [HttpPost("transportgroup")]
        public async Task<ActionResult<GetRosterDataResponse>> GetTransportGroupData(GetTransportGroupDataRequest request, CancellationToken cancellationToken)
        {

            var outData = new GetCampInfoResponse();
            string cacheEntityName = $"DashboardTransportAdmin_trasnportgroup_{request.currentDate?.ToString("yyyy-MM-dd")}_{request.departmentIds}";

            if (_memoryCache.TryGetValue($"API::{cacheEntityName}", out outData))
            {
                return Ok(outData);
            }
            else
            {
                var response = await _mediator.Send(request, cancellationToken);
                _memoryCache.Set($"API::{cacheEntityName}", response, TimeSpan.FromMinutes(GlobalConstants.ENDPOINT_TAS_DASHBOARD_CACHE_MINUTE));
                return Ok(response);

            }
        }


        [HttpPost("transportgroup/employees")]
        public async Task<ActionResult> GetTransportGroupEmployeeData(GetTransportGroupEmployeeDataRequest request, CancellationToken cancellationToken)
        {

            var response = await _mediator.Send(request, cancellationToken);
            return Ok(response);

        }


        [HttpGet("roster/month")]
        public async Task<ActionResult<GetRosterDataResponse>> GetRosterMonthlyData(DateTime? startDate, CancellationToken cancellationToken)
        {

            var outData = new GetCampInfoResponse();
            string cacheEntityName = $"DashboardTransportAdmin_roster_monthly_{startDate?.ToString("yyyy-MM-dd")}";

            if (_memoryCache.TryGetValue($"API::{cacheEntityName}", out outData))
            {
                return Ok(outData);
            }
            else
            {
                var response = await _mediator.Send(new GetRosterDataRequest(startDate, "MONTHLY"), cancellationToken);
                _memoryCache.Set($"API::{cacheEntityName}", response, TimeSpan.FromMinutes(GlobalConstants.ENDPOINT_TAS_DASHBOARD_CACHE_MINUTE));
                return Ok(response);

            }
        }

        [HttpGet("roster/quarter")]
        public async Task<ActionResult<GetRosterDataResponse>> GetRosterQuartlyData(DateTime? startDate, CancellationToken cancellationToken)
        {

            var outData = new GetCampInfoResponse();
            string cacheEntityName = $"DashboardTransportAdmin_roster_quartly_{startDate?.ToString("yyyy-MM-dd")}";

            if (_memoryCache.TryGetValue($"API::{cacheEntityName}", out outData))
            {
                return Ok(outData);
            }
            else
            {
                var response = await _mediator.Send(new GetRosterDataRequest(startDate, "QUARTLY"), cancellationToken);
                _memoryCache.Set($"API::{cacheEntityName}", response, TimeSpan.FromMinutes(GlobalConstants.ENDPOINT_TAS_DASHBOARD_CACHE_MINUTE));
                return Ok(response);

            }
        }


        [HttpGet("roster/year")]
        public async Task<ActionResult<GetRosterDataResponse>> GetRosterYearlyData(DateTime? startDate,  CancellationToken cancellationToken)
        {

            var outData = new GetCampInfoResponse();
            string cacheEntityName = $"DashboardTransportAdmin_roster_yearly_{startDate?.ToString("yyyy-MM-dd")}";

            if (_memoryCache.TryGetValue($"API::{cacheEntityName}", out outData))
            {
                return Ok(outData);
            }
            else
            {
                var response = await _mediator.Send(new GetRosterDataRequest(startDate, "YEARLY"), cancellationToken);
                _memoryCache.Set($"API::{cacheEntityName}", response, TimeSpan.FromMinutes(GlobalConstants.ENDPOINT_TAS_DASHBOARD_CACHE_MINUTE));
                return Ok(response);

            }
        }


        [HttpGet("domestic")]
        public async Task<ActionResult<GetDomesticDataResponse>> GetDomesticData(DateTime? startDate, CancellationToken cancellationToken)
        {

            var outData = new GetCampInfoResponse();
            string cacheEntityName = $"DashboardTransportAdmin_domestic_{startDate?.ToString("yyyy-MM-dd")}";

            if (_memoryCache.TryGetValue($"API::{cacheEntityName}", out outData))
            {
                return Ok(outData);
            }
            else
            {
                var response = await _mediator.Send(new GetDomesticDataRequest(startDate), cancellationToken);
                _memoryCache.Set($"API::{cacheEntityName}", response, TimeSpan.FromMinutes(GlobalConstants.ENDPOINT_TAS_DASHBOARD_CACHE_MINUTE));
                return Ok(response);

            }



        }
    }


}