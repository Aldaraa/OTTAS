using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using tas.Application.Features.EmployerFeature.GetAllEmployer;
using tas.Application.Features.ShiftFeature.CreateShift;
using tas.Application.Features.ShiftFeature.DeleteShift;
using tas.Application.Features.ShiftFeature.GetAllShift;
using tas.Application.Features.ShiftFeature.UpdateShift;
using tas.Application.Repositories;
using tas.Application.Service;
using tas.Domain.Common;
using tas.Domain.Entities;

namespace tas.WebAPI.Controllers.Tas
{

    [Route("api/tas/[controller]")]
    [ApiController]
    [RoleAuthorize]
    public class ShiftController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<ShiftController> _logger;
        private readonly CacheService _memoryCache;
        private readonly HTTPUserRepository _hTTPUserRepository;
        public ShiftController(IMediator mediator, ILogger<ShiftController> logger, CacheService memoryCache, HTTPUserRepository hTTPUserRepository)
        {
            _mediator = mediator;
            _logger = logger;
            _memoryCache = memoryCache;
            _hTTPUserRepository = hTTPUserRepository;   
        }

        [HttpGet]
        public async Task<ActionResult<List<GetAllShiftResponse>>> GetAll(int? active, CancellationToken cancellationToken)
        {

            var outData = new List<GetAllShiftResponse>();
            string cacheEntityName = typeof(Shift).Name;
            if (active == null)
            {
                if (_memoryCache.TryGetValue($"API::{cacheEntityName}", out outData))
                {
                    return Ok(outData);
                }
                else
                {

                    var response = await _mediator.Send(new GetAllShiftRequest(null), cancellationToken);
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
                    var response = await _mediator.Send(new GetAllShiftRequest(active), cancellationToken);
                    _memoryCache.Set($"API::{cacheEntityName}_{active}", response, TimeSpan.FromMinutes(GlobalConstants.ENDPOINT_MASTER_CACHE_MINUTE));
                    return Ok(response);

                }
            }



        }

        [HttpPost]
        public async Task<ActionResult> Create(CreateShiftRequest request,
            CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(request, cancellationToken);
            _hTTPUserRepository.ClearAllMasterCache<Shift>();
            //_memoryCache.Remove($"API::{typeof(Shift).Name}_1");
            //_memoryCache.Remove($"API::{typeof(Shift).Name}_0");
            //_memoryCache.Remove($"API::{typeof(Shift).Name}");
            return Ok(response);
        }


        [HttpPut]
        public async Task<ActionResult> Update(UpdateShiftRequest request,
            CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(request, cancellationToken);
            _hTTPUserRepository.ClearAllMasterCache<Shift>();
            return Ok(response);
        }

        [HttpDelete]
        public async Task<ActionResult> Delete(DeleteShiftRequest request,
            CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(request, cancellationToken);
            _hTTPUserRepository.ClearAllMasterCache<Shift>();
            return Ok(response);
        }

    }
}
