using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using tas.Application.Features.BusstopFeature.CreateBusstop;
using tas.Application.Features.BusstopFeature.DeleteBusstop;
using tas.Application.Features.BusstopFeature.GetAllBusstop;
using tas.Application.Features.BusstopFeature.UpdateBusstop;
using tas.Application.Service;
using tas.Domain.Common;
using tas.Domain.Entities;

namespace tas.WebAPI.Controllers.Tas
{
    [Route("api/tas/[controller]")]
    [ApiController]
    [RoleAuthorize]
    public class BusstopController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<BusstopController> _logger;
        private readonly IMemoryCache _memoryCache;

        public BusstopController(IMediator mediator, ILogger<BusstopController> logger, IMemoryCache memoryCache)
        {
            _mediator = mediator;
            _logger = logger;
            _memoryCache = memoryCache;
        }

        [HttpGet]
        public async Task<ActionResult<List<GetAllBusstopResponse>>> GetAll(CancellationToken cancellationToken)
        {

            var outData = new List<GetAllBusstopResponse>();
            string cacheEntityName = typeof(Busstop).Name;
            if (_memoryCache.TryGetValue($"API::{cacheEntityName}", out outData))
            {
                return Ok(outData);
            }
            else
            {

                var response = await _mediator.Send(new GetAllBusstopRequest(), cancellationToken);
                _memoryCache.Set($"API::{cacheEntityName}", response, TimeSpan.FromMinutes(GlobalConstants.ENDPOINT_MASTER_CACHE_MINUTE));
                return Ok(response);

            }
            
        }

        [HttpPost]
        public async Task<ActionResult> Create(CreateBusstopRequest request,
            CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(request, cancellationToken);
            _memoryCache.Remove($"API::{typeof(Busstop).Name}_1");
            _memoryCache.Remove($"API::{typeof(Busstop).Name}_0");
            _memoryCache.Remove($"API::{typeof(Busstop).Name}");
            return Ok(response);
        }


        [HttpPut]
        public async Task<ActionResult> Update(UpdateBusstopRequest request,
            CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(request, cancellationToken);
            _memoryCache.Remove($"API::{typeof(Busstop).Name}_1");
            _memoryCache.Remove($"API::{typeof(Busstop).Name}_0");
            _memoryCache.Remove($"API::{typeof(Busstop).Name}");
            return Ok(response);
        }

        [HttpDelete]
        public async Task<ActionResult> Delete(DeleteBusstopRequest request,
            CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(request, cancellationToken);
            _memoryCache.Remove($"API::{typeof(Busstop).Name}_1");
            _memoryCache.Remove($"API::{typeof(Busstop).Name}_0");
            _memoryCache.Remove($"API::{typeof(Busstop).Name}");
            return Ok(response);
        }

    }
}
