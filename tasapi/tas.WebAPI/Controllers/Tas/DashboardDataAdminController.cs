using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using tas.Application.Features.DashboardDataAdminFeature.GetEmployeeRegisterData;
using tas.Application.Features.DashboardDataAdminFeature.GetOnsiteEmployeesData;
using tas.Application.Features.DashboardDataAdminFeature.GetPackMealData;
using tas.Application.Features.DashboardDataAdminFeature.GetProfileChangeDepartmentData;
using tas.Application.Features.DashboardDataAdminFeature.GetSeatBlockOnsiteData;
using tas.Application.Features.DashboardDataAdminFeature.GetTransportData;
using tas.Application.Features.DashboardFeature.EmployeeDashboard;
using tas.Application.Features.DashboardFeature.RoomDashboard;
using tas.Application.Service;
using tas.Domain.Common;

namespace tas.WebAPI.Controllers.Tas
{
    [Route("api/tas/[controller]")]
    [ApiController]
    [RoleAuthorize]
    public class DashboardDataAdminController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<DashboardDataAdminController> _logger;
        private readonly CacheService _memoryCache;


        public DashboardDataAdminController(IMediator mediator, ILogger<DashboardDataAdminController> logger, CacheService memoryCache)
        {
            _mediator = mediator;
            _logger = logger;
            _memoryCache = memoryCache;
        }





        [HttpGet("packmeal")]
        public async Task<ActionResult<List<GetPackMealDataResponse>>> GetPackMealData(CancellationToken cancellationToken)
        {

            var outData = new List<GetPackMealDataResponse>();
            string cacheEntityName = $"DashboardDataAdmin_packmeal";

            if (_memoryCache.TryGetValue($"API::{cacheEntityName}", out outData))
            {
                return Ok(outData);
            }
            else
            {
                var response = await _mediator.Send(new GetPackMealDataRequest(), cancellationToken);
                _memoryCache.Set($"API::{cacheEntityName}", response, TimeSpan.FromMinutes(GlobalConstants.ENDPOINT_TAS_DASHBOARD_CACHE_MINUTE));
                return Ok(response);

            }


        }


        [HttpGet("employeeregister")]
        public async Task<ActionResult<List<GetEmployeeRegisterDataResponse>>> GetEmployeeRegisterData(DateTime? startDate, CancellationToken cancellationToken)
        {

            var outData = new List<GetEmployeeRegisterDataResponse>();
            string cacheEntityName = $"DashboardDataAdmin_employeeregister_{startDate:yyyyMMdd}";

            if (_memoryCache.TryGetValue($"API::{cacheEntityName}", out outData))
            {
                return Ok(outData);
            }
            else
            {
                var response = await _mediator.Send(new GetEmployeeRegisterDataRequest(startDate), cancellationToken);
                _memoryCache.Set($"API::{cacheEntityName}", response, TimeSpan.FromMinutes(GlobalConstants.ENDPOINT_TAS_DASHBOARD_CACHE_MINUTE));
                return Ok(response);

            }


        }


        [HttpGet("onsiteemployees")]
        public async Task<ActionResult<List<GetOnsiteEmployeesDataResponse>>> GetEmployeeOnsiteData(CancellationToken cancellationToken)
        {

            var outData = new List<GetOnsiteEmployeesDataResponse>();
            string cacheEntityName = $"DashboardDataAdmin_onsiteemployees";

            if (_memoryCache.TryGetValue($"API::{cacheEntityName}", out outData))
            {
                return Ok(outData);
            }
            else
            {
                var response = await _mediator.Send(new GetOnsiteEmployeesDataRequest(), cancellationToken);
                _memoryCache.Set($"API::{cacheEntityName}", response, TimeSpan.FromMinutes(GlobalConstants.ENDPOINT_TAS_DASHBOARD_CACHE_MINUTE));
                return Ok(response);

            }


        }



        [HttpGet("seatblockonsiteemployees")]
        public async Task<ActionResult<List<GetSeatBlockOnsiteDataResponse>>> GetSeatBlockEmployeeOnsiteData(DateTime? startDate, CancellationToken cancellationToken)
        {


            var outData = new List<GetSeatBlockOnsiteDataResponse>();
            string cacheEntityName = $"DashboardDataAdmin_seatblockonsiteemployees_{startDate:yyyyMMdd}"; 

            if (_memoryCache.TryGetValue($"API::{cacheEntityName}", out outData))
            {
                return Ok(outData);
            }
            else
            {
                var response = await _mediator.Send(new GetSeatBlockOnsiteDataRequest(startDate), cancellationToken);
                _memoryCache.Set($"API::{cacheEntityName}", response, TimeSpan.FromMinutes(GlobalConstants.ENDPOINT_TAS_DASHBOARD_CACHE_MINUTE));

                return Ok(response);
            }


        }




        [HttpGet("profilechangedata")]
        public async Task<ActionResult<GetProfileChangeDepartmentDataResponse>> GetProfileChangeDepartmentData(DateTime? startDate, DateTime? endDate, CancellationToken cancellationToken)
        {
            var outData = new GetProfileChangeDepartmentDataResponse();
            string cacheEntityName = $"DashboardDataAdmin_profilechangedata_{startDate:yyyyMMdd}_{endDate:yyyyMMdd}";

            if (_memoryCache.TryGetValue($"API::{cacheEntityName}", out outData))
            {
                return Ok(outData);
            }
            else
            {
                var response = await _mediator.Send(new GetProfileChangeDepartmentDataRequest(startDate, endDate), cancellationToken);
                _memoryCache.Set($"API::{cacheEntityName}", response, TimeSpan.FromMinutes(GlobalConstants.ENDPOINT_TAS_DASHBOARD_CACHE_MINUTE));
                return Ok(response);
            }


        }


        [HttpGet("transportdata")]
        public async Task<ActionResult<GetTransportDataResponse>> GetTransportData(DateTime? startDate, CancellationToken cancellationToken)
        {
            var outData = new GetTransportDataResponse();
            string cacheEntityName = $"DashboardDataAdmin_transportdata";

            if (_memoryCache.TryGetValue($"API::{cacheEntityName}", out outData))
            {
                return Ok(outData);
            }
            else
            {
                var response = await _mediator.Send(new GetTransportDataRequest(startDate), cancellationToken);
                _memoryCache.Set($"API::{cacheEntityName}", response, TimeSpan.FromMinutes(GlobalConstants.ENDPOINT_TAS_DASHBOARD_CACHE_MINUTE));
                return Ok(response);
            }


        }

    }

}
