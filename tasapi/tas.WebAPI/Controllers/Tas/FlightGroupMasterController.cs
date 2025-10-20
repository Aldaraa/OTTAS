using DocumentFormat.OpenXml.Wordprocessing;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using tas.Application.Features.FlightGroupMasterFeature.CreateFlightGroupMaster;
using tas.Application.Features.FlightGroupMasterFeature.DeleteFlightGroupMaster;
using tas.Application.Features.FlightGroupMasterFeature.GetAllFlightGroupMaster;
using tas.Application.Features.FlightGroupMasterFeature.GetFlightGroupMaster;
using tas.Application.Features.FlightGroupMasterFeature.UpdateFlightGroupMaster;
using tas.Application.Features.NationalityFeature.GetAllNationality;
using tas.Application.Service;
using tas.Domain.Common;
using tas.Domain.Entities;

namespace tas.WebAPI.Controllers.Tas
{


    [Route("api/tas/[controller]")]
    [ApiController]
    [RoleAuthorize]
    public class FlightGroupMasterController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<FlightGroupMasterController> _logger;
        private readonly CacheService _memoryCache;

        public FlightGroupMasterController(IMediator mediator, ILogger<FlightGroupMasterController> logger, CacheService memoryCache)
        {
            _mediator = mediator;
            _logger = logger;
            _memoryCache = memoryCache;
        }

        [HttpGet]
        public async Task<ActionResult<List<GetAllFlightGroupMasterResponse>>> GetAll(int? active, int? fullcluster, CancellationToken cancellationToken)
        {

            var outData = new List<GetAllFlightGroupMasterResponse>();
            string cacheEntityName = typeof(FlightGroupMaster).Name;
            if (active == null)
            {
                if (_memoryCache.TryGetValue($"API::{cacheEntityName}", out outData))
                {
                    return Ok(outData);
                }
                else
                {

                    var response = await _mediator.Send(new GetAllFlightGroupMasterRequest(null, null), cancellationToken);
                    _memoryCache.Set($"API::{cacheEntityName}", response, TimeSpan.FromMinutes(GlobalConstants.ENDPOINT_MASTER_CACHE_MINUTE));
                    return Ok(response);

                }
            }
            else
            {
                if (_memoryCache.TryGetValue($"API::{cacheEntityName}_{active}", out outData))
                {
                    return Ok(outData);
                }
                else
                {
                    var response = await _mediator.Send(new GetAllFlightGroupMasterRequest(active, fullcluster), cancellationToken);
                    _memoryCache.Set($"API::{cacheEntityName}_{active}", response, TimeSpan.FromMinutes(GlobalConstants.ENDPOINT_MASTER_CACHE_MINUTE));
                    return Ok(response);

                }
            }



        }


        [HttpGet("{Id}")]
        public async Task<ActionResult<GetFlightGroupMasterResponse>> Get(int Id, CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(new GetFlightGroupMasterRequest(Id), cancellationToken);

            return Ok(response);

        }

        [HttpPost]
        public async Task<ActionResult> Create(CreateFlightGroupMasterRequest request,
            CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(request, cancellationToken);
            _memoryCache.Remove($"API::{typeof(FlightGroupMaster).Name}_1");
            _memoryCache.Remove($"API::{typeof(FlightGroupMaster).Name}_0");
            _memoryCache.Remove($"API::{typeof(FlightGroupMaster).Name}");
            return Ok(response);
        }


        [HttpPut]
        public async Task<ActionResult> Update(UpdateFlightGroupMasterRequest request,
            CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(request, cancellationToken);
            _memoryCache.Remove($"API::{typeof(FlightGroupMaster).Name}_1");
            _memoryCache.Remove($"API::{typeof(FlightGroupMaster).Name}_0");
            _memoryCache.Remove($"API::{typeof(FlightGroupMaster).Name}");
            return Ok(response);
        }

        [HttpDelete]
        public async Task<ActionResult> Delete(DeleteFlightGroupMasterRequest request,
            CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(request, cancellationToken);
            _memoryCache.Remove($"API::{typeof(FlightGroupMaster).Name}_1");
            _memoryCache.Remove($"API::{typeof(FlightGroupMaster).Name}_0");
            _memoryCache.Remove($"API::{typeof(FlightGroupMaster).Name}");
            return Ok(response);
        }

    }
}
