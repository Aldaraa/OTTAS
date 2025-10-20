using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using tas.Application.Features.RosterFeature.GetAllRoster;
using tas.Application.Features.RosterGroupFeature.CreateRosterGroup;
using tas.Application.Features.RosterGroupFeature.DeleteRosterGroup;
using tas.Application.Features.RosterGroupFeature.GetAllRosterGroup;
using tas.Application.Features.RosterGroupFeature.UpdateRosterGroup;
using tas.Application.Service;
using tas.Domain.Common;
using tas.Domain.Entities;

namespace tas.WebAPI.Controllers.Tas
{
    [Route("api/tas/[controller]")]
    [ApiController]
    [RoleAuthorize]
    public class RosterGroupController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<RosterGroupController> _logger;
        private readonly CacheService _memoryCache;
        public RosterGroupController(IMediator mediator, ILogger<RosterGroupController> logger, CacheService memoryCache)
        {
            _mediator = mediator;
            _logger = logger;
            _memoryCache = memoryCache; 
        }

        //[HttpGet]
        //public async Task<ActionResult<List<GetAllRosterGroupResponse>>> GetAll(int? active, CancellationToken cancellationToken)
        //{
        //    if (active == null)
        //    {
        //        var response = await _mediator.Send(new GetAllRosterGroupRequest(null), cancellationToken);
        //        return Ok(response);
        //    }
        //    else
        //    {
        //        var response = await _mediator.Send(new GetAllRosterGroupRequest(active), cancellationToken);
        //        return Ok(response);

        //    }
        //}

        [HttpGet]
        public async Task<ActionResult<List<GetAllRosterGroupResponse>>> GetAll(int? active, CancellationToken cancellationToken)
        {

            var outData = new List<GetAllRosterGroupResponse>();
            string cacheEntityName = typeof(Roster).Name;
            if (active == null)
            {
                if (_memoryCache.TryGetValue($"API::{cacheEntityName}", out outData))
                {
                    return Ok(outData);
                }
                else
                {

                    var response = await _mediator.Send(new GetAllRosterGroupRequest(null), cancellationToken);
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
                    var response = await _mediator.Send(new GetAllRosterGroupRequest(active), cancellationToken);
                    _memoryCache.Set($"API::{cacheEntityName}_{active}", response, TimeSpan.FromMinutes(GlobalConstants.ENDPOINT_MASTER_CACHE_MINUTE));
                    return Ok(response);

                }
            }

        }

        [HttpPost]
        public async Task<ActionResult> Create(CreateRosterGroupRequest request,
            CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(request, cancellationToken);
            return Ok(response);
        }


        [HttpPut]
        public async Task<ActionResult> Update(UpdateRosterGroupRequest request,
            CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(request, cancellationToken);
            return Ok(response);
        }

        [HttpDelete]
        public async Task<ActionResult> Delete(DeleteRosterGroupRequest request,
            CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(request, cancellationToken);
            return Ok(response);
        }

    }
}
