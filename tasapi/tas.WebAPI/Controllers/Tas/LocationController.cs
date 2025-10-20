using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using tas.Application.Features.LocationFeature.CreateLocation;
using tas.Application.Features.LocationFeature.DeleteLocation;
using tas.Application.Features.LocationFeature.GetAllLocation;
using tas.Application.Features.LocationFeature.UpdateLocation;
using tas.Application.Features.StateFeature.GetAllState;
using tas.Application.Repositories;
using tas.Application.Service;
using tas.Domain.Common;
using tas.Domain.Entities;

namespace tas.WebAPI.Controllers.Tas
{

    [Route("api/tas/[controller]")]
    [ApiController]
    [RoleAuthorize]
    public class LocationController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<LocationController> _logger;
        private readonly CacheService _memoryCache;
        private readonly  HTTPUserRepository _HTTPUserRepository;


        public LocationController(IMediator mediator, ILogger<LocationController> logger, CacheService memoryCache, HTTPUserRepository hTTPUserRepository)
        {
            _mediator = mediator;
            _logger = logger;
            _memoryCache = memoryCache; 
            _HTTPUserRepository = hTTPUserRepository;   
        }

        [HttpGet]
        public async Task<ActionResult<List<GetAllLocationResponse>>> GetAll(int? active, CancellationToken cancellationToken)
        {

            var outData = new List<GetAllLocationResponse>();
            string cacheEntityName = typeof(Location).Name;
            if (active == null)
            {
                if (_memoryCache.TryGetValue($"API::{cacheEntityName}", out outData))
                {
                    return Ok(outData);
                }
                else
                {

                    var response = await _mediator.Send(new GetAllLocationRequest(null), cancellationToken);
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
                    var response = await _mediator.Send(new GetAllLocationRequest(active), cancellationToken);
                    _memoryCache.Set($"API::{cacheEntityName}_{active}", response, TimeSpan.FromMinutes(GlobalConstants.ENDPOINT_MASTER_CACHE_MINUTE));
                    return Ok(response);

                }
            }



        }

        [HttpPost]
        public async Task<ActionResult> Create(CreateLocationRequest request,
            CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(request, cancellationToken);
            _HTTPUserRepository.ClearAllMasterCache<Location>();
            return Ok(response);
        }


        [HttpPut]
        public async Task<ActionResult> Update(UpdateLocationRequest request,
            CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(request, cancellationToken);
            _HTTPUserRepository.ClearAllMasterCache<Location>();
            return Ok(response);
        }

        [HttpDelete]
        public async Task<ActionResult> Delete(DeleteLocationRequest request,
            CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(request, cancellationToken);
            _HTTPUserRepository.ClearAllMasterCache<Location>();
            return Ok(response);
        }

    }
}
