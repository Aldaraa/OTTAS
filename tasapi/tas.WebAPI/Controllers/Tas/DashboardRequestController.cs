using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using tas.Application.Features.DashboardRequestFeature.GetDocumentDashboard;
using tas.Application.Service;
using tas.Domain.Common;

namespace tas.WebAPI.Controllers.Tas
{

    [Route("api/tas/[controller]")]
    [ApiController]
    [RoleAuthorize]
    public class DashboardRequestController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<DashboardRequestController> _logger;
        private readonly CacheService _memoryCache;


        public DashboardRequestController(IMediator mediator, ILogger<DashboardRequestController> logger, CacheService memoryCache)
        {
            _mediator = mediator;
            _logger = logger;
            _memoryCache = memoryCache;
        }



        [HttpGet("documentdata")]
        public async Task<ActionResult<GetDocumentDashboardResponse>> GetDashboard(DateTime? startDate, DateTime? endDate, CancellationToken cancellationToken)
        {
            var outData = new GetDocumentDashboardResponse();
            string cacheEntityName = "Request_Dashboard";
            string cacheKey = $"API::{cacheEntityName}::{startDate?.ToString("yyyyMMdd") ?? "null"}::{endDate?.ToString("yyyyMMdd") ?? "null"}";

            if (_memoryCache.TryGetValue(cacheKey, out outData))
            {
                return Ok(outData);
            }
            else
            {
                var response = await _mediator.Send(new GetDocumentDashboardRequest(startDate, endDate), cancellationToken);
                _memoryCache.Set(cacheKey, response, TimeSpan.FromMinutes(GlobalConstants.ENDPOINT_TAS_DASHBOARD_CACHE_MINUTE));
                return Ok(response);
            }
        }

    }

 }
