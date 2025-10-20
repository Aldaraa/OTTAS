using DocumentFormat.OpenXml.InkML;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using Microsoft.Extensions.Caching.Memory;
using tas.Application.Features.PeopleTypeFeature.CreatePeopleType;
using tas.Application.Features.PeopleTypeFeature.DeletePeopleType;
using tas.Application.Features.PeopleTypeFeature.GetAllPeopleType;
using tas.Application.Features.PeopleTypeFeature.UpdatePeopleType;
using tas.Application.Repositories;
using tas.Application.Service;
using tas.Domain.Common;
using tas.Domain.Entities;

namespace tas.WebAPI.Controllers.Tas
{

    [Route("api/tas/[controller]")]
    [ApiController]
    [RoleAuthorize]
    public class PeopleTypeController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<PeopleTypeController> _logger;
        private readonly CacheService _memoryCache;

        private readonly HTTPUserRepository _hTTPUserRepository;

        public PeopleTypeController(IMediator mediator, ILogger<PeopleTypeController> logger, CacheService memoryCache, HTTPUserRepository hTTPUserRepository)
        {
            _mediator = mediator;
            _memoryCache = memoryCache;
            _logger = logger;
            _hTTPUserRepository = hTTPUserRepository;   
        }

        [HttpGet]
        public async Task<ActionResult<List<GetAllPeopleTypeResponse>>> GetAll(int? active, CancellationToken cancellationToken)
        {
            if (active == null)
            {
                var outData = new List<GetAllPeopleTypeResponse>();
                if (_memoryCache.TryGetValue($"API::{typeof(PeopleType).Name}", out outData))
                {
                    return Ok(outData);
                }
                else
                {

                    var response = await _mediator.Send(new GetAllPeopleTypeRequest(null), cancellationToken);
                    _memoryCache.Set($"API::{typeof(PeopleType).Name}", response, TimeSpan.FromMinutes(GlobalConstants.ENDPOINT_MASTER_CACHE_MINUTE));
                    return Ok(response);

                }
            }
            else
            {
                var outData = new List<GetAllPeopleTypeResponse>();
                if (_memoryCache.TryGetValue($"API::{typeof(PeopleType).Name}_{active}", out outData))
                {
                    return Ok(outData);
                }
                else
                {
                    var response = await _mediator.Send(new GetAllPeopleTypeRequest(active), cancellationToken);
                    _memoryCache.Set($"API::{typeof(PeopleType).Name}_{active}", response, TimeSpan.FromMinutes(GlobalConstants.ENDPOINT_MASTER_CACHE_MINUTE));
                    return Ok(response);

                }
            }
        }

        [HttpPost]
        public async Task<ActionResult> Create(CreatePeopleTypeRequest request,
            CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(request, cancellationToken);
            _hTTPUserRepository.ClearAllMasterCache<PeopleType>();
            return Ok(response);
        }


        [HttpPut]
        public async Task<ActionResult> Update(UpdatePeopleTypeRequest request,
            CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(request, cancellationToken);
            _hTTPUserRepository.ClearAllMasterCache<PeopleType>();
            return Ok(response);
        }

        [HttpDelete]
        public async Task<ActionResult> Delete(DeletePeopleTypeRequest request,
            CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(request, cancellationToken);
            _hTTPUserRepository.ClearAllMasterCache<PeopleType>();
            //_memoryCache.Remove($"API::{typeof(PeopleType).Name}");
            //_memoryCache.Remove($"API::{typeof(PeopleType).Name}_1");
            //_memoryCache.Remove($"API::{typeof(PeopleType).Name}_0");
            return Ok(response);
        }

    }
}
