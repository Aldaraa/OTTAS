using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using tas.Application.Features.DashboardDataAdminFeature.GetPackMealData;
using tas.Application.Features.DashboardSystemAdminFeature.GeOnsiteEmployeesData;
using tas.Application.Features.DashboardSystemAdminFeature.GetCampPOBData;
using tas.Application.Features.DashboardSystemAdminFeature.GetCampUtilizationData;
using tas.Application.Features.DashboardSystemAdminFeature.GetEmployeeTransportData;
using tas.Application.Features.DashboardSystemAdminFeature.GetPeopleTypeAndDepartment;
using tas.Application.Features.DashboardSystemAdminFeature.GetStatData;
using tas.Application.Service;
using tas.Domain.Common;

namespace tas.WebAPI.Controllers.Tas
{

    [Route("api/tas/[controller]")]
    [ApiController]
    [RoleAuthorize]
    public class DashboardSystemAdminController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<DashboardSystemAdminController> _logger;
        private readonly CacheService _memoryCache;


        public DashboardSystemAdminController(IMediator mediator, ILogger<DashboardSystemAdminController> logger, CacheService memoryCache)
        {
            _mediator = mediator;
            _logger = logger;
            _memoryCache = memoryCache;
        }





        [HttpGet("onsiteemployees")]
        public async Task<ActionResult> GetOnsiteEmployees(DateTime? startDate, CancellationToken cancellationToken)
        {
            var outData = new GeOnsiteEmployeesDataResponse();
            string cacheEntityName = $"DashboardSystemAdmin_onsiteemployees";


            if (_memoryCache.TryGetValue($"API::{cacheEntityName}", out outData))
            {
                // Log successful cache retrieval
                Console.WriteLine("from cache");
                return Ok(outData);
            }
            else
            {
                // Log cache miss
                Console.WriteLine("from db");

                var response = await _mediator.Send(new GeOnsiteEmployeesDataRequest(startDate), cancellationToken);
                _memoryCache.Set($"API::{cacheEntityName}", response, TimeSpan.FromMinutes(GlobalConstants.ENDPOINT_TAS_DASHBOARD_CACHE_MINUTE));
                return Ok(response);
            }


        }


        [HttpGet("statdata")]
        public async Task<ActionResult> GetServerStat(CancellationToken cancellationToken)
        {

            var outData = new List<GetStatDataResponse>();
            string cacheEntityName = $"DashboardSystemAdmin_statdata";

            if (_memoryCache.TryGetValue($"API::{cacheEntityName}", out outData))
            {
                return Ok(outData);
            }
            else
            {
                var response = await _mediator.Send(new GetStatDataRequest(), cancellationToken);
                _memoryCache.Set($"API::{cacheEntityName}", response, TimeSpan.FromMinutes(GlobalConstants.ENDPOINT_TAS_DASHBOARD_CACHE_MINUTE));
                return Ok(response);

            }


        }



        [HttpGet("employeecount")]
        public async Task<ActionResult> GetEmployeeCount(CancellationToken cancellationToken)
        {

            var outData = new GetPeopleTypeAndDepartmentResponse();
            string cacheEntityName = $"DashboardSystemAdmin_employeecount";

            if (_memoryCache.TryGetValue($"API::{cacheEntityName}", out outData))
            {
                return Ok(outData);
            }
            else
            {
                var response = await _mediator.Send(new GetPeopleTypeAndDepartmentRequest(), cancellationToken);
                _memoryCache.Set($"API::{cacheEntityName}", response, TimeSpan.FromMinutes(GlobalConstants.ENDPOINT_TAS_DASHBOARD_CACHE_MINUTE));
                return Ok(response);

            }


        }


        [HttpGet("camppob/weekly")]
        public async Task<ActionResult> GetCampWeekly(CancellationToken cancellationToken)
        {

            var outData = new List<GetCampPOBDataResponse>();
            string cacheEntityName = $"DashboardSystemAdmin_camppob_weekly";

            if (_memoryCache.TryGetValue($"API::{cacheEntityName}", out outData))
            {
                return Ok(outData);
            }
            else
            {
                var response = await _mediator.Send(new GetCampPOBDataRequest(true), cancellationToken);
                _memoryCache.Set($"API::{cacheEntityName}", response, TimeSpan.FromMinutes(GlobalConstants.ENDPOINT_TAS_DASHBOARD_CACHE_MINUTE));
                return Ok(response);

            }


        }

        [HttpGet("camppob/monthly")]
        public async Task<ActionResult> GetCampPOBmonthly(CancellationToken cancellationToken)
        {

            var outData = new List<GetCampPOBDataResponse>();
            string cacheEntityName = $"DashboardSystemAdmin_camppob_monthly";

            if (_memoryCache.TryGetValue($"API::{cacheEntityName}", out outData))
            {
                return Ok(outData);
            }
            else
            {
                var response = await _mediator.Send(new GetCampPOBDataRequest(false), cancellationToken);
                _memoryCache.Set($"API::{cacheEntityName}", response, TimeSpan.FromMinutes(GlobalConstants.ENDPOINT_TAS_DASHBOARD_CACHE_MINUTE));
                return Ok(response);

            }


        }

        /// <param name="type">The type of utilization data to retrieve. Valid values are 'Daily', 'Weekly', 'Monthly'.</param>
        /// <returns>A list of camp utilization data.</returns>
        [HttpGet("campusage/{type}")]
        public async Task<ActionResult> GetCampUsage(string type, CancellationToken cancellationToken)
        {

            var outData = new List<GetCampUtilizationDataResponse>();
            string cacheEntityName = $"DashboardSystemAdmin_campusage_{type}";

            if (_memoryCache.TryGetValue($"API::{cacheEntityName}", out outData))
            {
                return Ok(outData);
            }
            else
            {
                var response = await _mediator.Send(new GetCampUtilizationDataRequest(type), cancellationToken);
                _memoryCache.Set($"API::{cacheEntityName}", response, TimeSpan.FromMinutes(GlobalConstants.ENDPOINT_TAS_DASHBOARD_CACHE_MINUTE));
                return Ok(response);

            }


        }

        [HttpGet("locationdata/{date}")]
        public async Task<ActionResult> GetLocationData(DateTime date, CancellationToken cancellationToken)
        {

            var outData = new GetEmployeeTransportDataResponse();
            string cacheEntityName = $"DashboardSystemAdmin_locationdata_{date.ToString("yyyy-MM-dd")}";

            if (_memoryCache.TryGetValue($"API::{cacheEntityName}", out outData))
            {
                return Ok(outData);
            }
            else
            {
                var response = await _mediator.Send(new GetEmployeeTransportDataRequest(date), cancellationToken);
                _memoryCache.Set($"API::{cacheEntityName}", response, TimeSpan.FromMinutes(GlobalConstants.ENDPOINT_TAS_DASHBOARD_CACHE_MINUTE));
                return Ok(response);

            }


        }
    }

}
