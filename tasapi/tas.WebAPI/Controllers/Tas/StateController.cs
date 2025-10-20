using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using tas.Application.Features.ShiftFeature.GetAllShift;
using tas.Application.Features.StateFeature.CreateState;
using tas.Application.Features.StateFeature.DeleteState;
using tas.Application.Features.StateFeature.GetAllState;
using tas.Application.Features.StateFeature.UpdateState;
using tas.Application.Service;
using tas.Domain.Common;
using tas.Domain.Entities;

namespace tas.WebAPI.Controllers.Tas
{

    [Route("api/tas/[controller]")]
    [ApiController]
    [RoleAuthorize]
    public class StateController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<StateController> _logger;
        private readonly IMemoryCache _memoryCache;

        public StateController(IMediator mediator, ILogger<StateController> logger, IMemoryCache memoryCache)
        {
            _mediator = mediator;
            _logger = logger;
            _memoryCache = memoryCache;
        }

        [HttpGet]
        public async Task<ActionResult<List<GetAllStateResponse>>> GetAll(int? active, CancellationToken cancellationToken)
        {
            var outData = new List<GetAllStateResponse>();
            string cacheEntityName = typeof(State).Name;
            if (active == null)
            {
                if (_memoryCache.TryGetValue($"API::{cacheEntityName}", out outData))
                {
                    return Ok(outData);
                }
                else
                {

                    var response = await _mediator.Send(new GetAllStateRequest(null), cancellationToken);
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
                    var response = await _mediator.Send(new GetAllStateRequest(active), cancellationToken);
                    _memoryCache.Set($"API::{cacheEntityName}_{active}", response, TimeSpan.FromMinutes(GlobalConstants.ENDPOINT_MASTER_CACHE_MINUTE));
                    return Ok(response);

                }
            }



        }

        [HttpPost]
        public async Task<ActionResult> Create(CreateStateRequest request,
            CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(request, cancellationToken);
            _memoryCache.Remove($"API::{typeof(State).Name}_1");
            _memoryCache.Remove($"API::{typeof(State).Name}_0");
            _memoryCache.Remove($"API::{typeof(State).Name}");
            return Ok(response);
        }


        [HttpPut]
        public async Task<ActionResult> Update(UpdateStateRequest request,
            CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(request, cancellationToken);
            _memoryCache.Remove($"API::{typeof(State).Name}_1");
            _memoryCache.Remove($"API::{typeof(State).Name}_0");
            _memoryCache.Remove($"API::{typeof(State).Name}");
            return Ok(response);
        }

        [HttpDelete]
        public async Task<ActionResult> Delete(DeleteStateRequest request,
            CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(request, cancellationToken);
            _memoryCache.Remove($"API::{typeof(State).Name}_1");
            _memoryCache.Remove($"API::{typeof(State).Name}_0");
            _memoryCache.Remove($"API::{typeof(State).Name}");
            return Ok(response);
        }



    }
}
