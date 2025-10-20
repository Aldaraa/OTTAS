using DocumentFormat.OpenXml.Wordprocessing;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using System;
using tas.Application.Features.DashboardFeature.EmployeeDashboard;
using tas.Application.Features.DashboardFeature.RoomDashboard;
using tas.Application.Features.DashboardFeature.TransportDashboard;
using tas.Application.Features.DepartmentFeature.GetAllDepartment;
using tas.Application.Features.PositionFeature.GetAllRoomType;
using tas.Application.Features.RoomTypeFeature.GetAllRoomType;
using tas.Application.Service;
using tas.Domain.Common;
using tas.Domain.Entities;

namespace tas.WebAPI.Controllers.Tas
{

    [Route("api/tas/[controller]")]
    [ApiController]
    [RoleAuthorize]
    public class DashboardController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<DashboardController> _logger;
        private readonly CacheService _memoryCache;


        public DashboardController(IMediator mediator, ILogger<DashboardController> logger, CacheService memoryCache)
        {
            _mediator = mediator;
            _logger = logger;
            _memoryCache = memoryCache;
        }



        [HttpGet("employee")]
        public async Task<ActionResult<EmployeeDashboardResponse>> EmployeeData( CancellationToken cancellationToken)
        {
            var outData = new EmployeeDashboardResponse();
            string cacheEntityName = "Dashboard_Employee";

            if (_memoryCache.TryGetValue($"API::{cacheEntityName}", out outData))
            {
                return Ok(outData);
            }
            else
            {
                var response = await _mediator.Send(new EmployeeDashboardRequest(null), cancellationToken);

                _memoryCache.Set($"API::{cacheEntityName}", response, TimeSpan.FromMinutes(GlobalConstants.ENDPOINT_TAS_DASHBOARD_CACHE_MINUTE));
                return Ok(response);

            }

        }

        [HttpGet("transport")]
        public async Task<ActionResult<EmployeeDashboardResponse>> TransportData(CancellationToken cancellationToken)
        {
            //var response = await _mediator.Send(new TransportDashboardRequest(null), cancellationToken);
            //return Ok(response);


            var outData = new TransportDashboardResponse();
            string cacheEntityName = "Dashboard_Transport";

            if (_memoryCache.TryGetValue($"API::{cacheEntityName}", out outData))
            {
                return Ok(outData);
            }
            else
            {
                var response = await _mediator.Send(new TransportDashboardRequest(null), cancellationToken);
                _memoryCache.Set($"API::{cacheEntityName}", response, TimeSpan.FromMinutes(GlobalConstants.ENDPOINT_TAS_DASHBOARD_CACHE_MINUTE));
                return Ok(response);

            }

        }


        [HttpGet("room")]
        public async Task<ActionResult<RoomDashboardResponse>> RoomData(CancellationToken cancellationToken)
        {

            var outData = new RoomDashboardResponse();
            string cacheEntityName = "Dashboard_Room";

            if (_memoryCache.TryGetValue($"API::{cacheEntityName}", out outData))
            {
                return Ok(outData);
            }
            else
            {
                var response = await _mediator.Send(new RoomDashboardRequest(null), cancellationToken);

                _memoryCache.Set($"API::{cacheEntityName}", response, TimeSpan.FromMinutes(GlobalConstants.ENDPOINT_TAS_DASHBOARD_CACHE_MINUTE));
                return Ok(response);

            }


        }

    }

}

