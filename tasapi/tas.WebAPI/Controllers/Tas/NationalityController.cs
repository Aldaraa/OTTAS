using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using tas.Application.Features.LocationFeature.GetAllLocation;
using tas.Application.Features.NationalityFeature.CreateNationality;
using tas.Application.Features.NationalityFeature.DeleteNationality;
using tas.Application.Features.NationalityFeature.GetAllNationality;
using tas.Application.Features.NationalityFeature.UpdateNationality;
using tas.Application.Repositories;
using tas.Application.Service;
using tas.Domain.Common;
using tas.Domain.Entities;

namespace tas.WebAPI.Controllers.Tas
{
    [Route("api/tas/[controller]")]
    [ApiController]
    [RoleAuthorize]
    public class NationalityController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<NationalityController> _logger;
        private readonly CacheService _memoryCache;

        private readonly HTTPUserRepository _hTTPUserRepository;


        public NationalityController(IMediator mediator, ILogger<NationalityController> logger, CacheService memoryCache, HTTPUserRepository hTTPUserRepository)
        {
            _mediator = mediator;
            _logger = logger;
            _memoryCache = memoryCache;
            _hTTPUserRepository = hTTPUserRepository;   
        }

        [HttpGet]
        public async Task<ActionResult<List<GetAllNationalityResponse>>> GetAll(int? active, CancellationToken cancellationToken)
        {
            var outData = new List<GetAllNationalityResponse>();
            string cacheEntityName = typeof(Nationality).Name;
            if (active == null)
            {
                if (_memoryCache.TryGetValue($"API::{cacheEntityName}", out outData))
                {
                    return Ok(outData);
                }
                else
                {

                    var response = await _mediator.Send(new GetAllNationalityRequest(null), cancellationToken);
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
                    var response = await _mediator.Send(new GetAllNationalityRequest(active), cancellationToken);
                    _memoryCache.Set($"API::{cacheEntityName}_{active}", response, TimeSpan.FromMinutes(GlobalConstants.ENDPOINT_MASTER_CACHE_MINUTE));
                    return Ok(response);

                }
            }


        }

        [HttpPost]
        public async Task<ActionResult> Create(CreateNationalityRequest request,
            CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(request, cancellationToken);
            _hTTPUserRepository.ClearAllMasterCache<Nationality>();
            //_memoryCache.Remove($"API::{typeof(Nationality).Name}_1");
            //_memoryCache.Remove($"API::{typeof(Nationality).Name}_0");
            //_memoryCache.Remove($"API::{typeof(Nationality).Name}");
            return Ok(response);
        }


        [HttpPut]
        public async Task<ActionResult> Update(UpdateNationalityRequest request,
            CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(request, cancellationToken);
            _hTTPUserRepository.ClearAllMasterCache<Nationality>();
            return Ok(response);
        }

        [HttpDelete]
        public async Task<ActionResult> Delete(DeleteNationalityRequest request,
            CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(request, cancellationToken);
            _hTTPUserRepository.ClearAllMasterCache<Nationality>();
            return Ok(response);
        }

    }
}
