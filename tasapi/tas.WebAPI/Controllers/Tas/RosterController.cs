using DocumentFormat.OpenXml.Wordprocessing;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using tas.Application.Features.CampFeature.GetAllCamp;
using tas.Application.Features.RosterFeature.CreateRoster;
using tas.Application.Features.RosterFeature.DeleteRoster;
using tas.Application.Features.RosterFeature.GetAllRoster;
using tas.Application.Features.RosterFeature.GetRoster;
using tas.Application.Features.RosterFeature.UpdateRoster;
using tas.Application.Features.ShiftFeature.GetAllShift;
using tas.Application.Service;
using tas.Domain.Common;
using tas.Domain.Entities;

namespace tas.WebAPI.Controllers.Tas
{
    [Route("api/tas/[controller]")]
    [ApiController]
    [RoleAuthorize]
    public class  RosterController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<RosterController> _logger;
        private readonly IMemoryCache _memoryCache;

        public RosterController(IMediator mediator, ILogger<RosterController> logger, IMemoryCache memoryCache)
        {
            _mediator = mediator;
            _logger = logger;
            _memoryCache = memoryCache;
        }

        [HttpGet]
        public async Task<ActionResult<List<GetAllRosterResponse>>> GetAll(int? active,  CancellationToken cancellationToken)
        {

            var outData = new List<GetAllRosterResponse>();
            string cacheEntityName = typeof(Roster).Name;
            if (active == null)
            {
                if (_memoryCache.TryGetValue($"API::{cacheEntityName}", out outData))
                {
                    return Ok(outData);
                }
                else
                {

                    var response = await _mediator.Send(new GetAllRosterRequest(null), cancellationToken);
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
                    var response = await _mediator.Send(new GetAllRosterRequest(active), cancellationToken);
                    _memoryCache.Set($"API::{cacheEntityName}_{active}", response, TimeSpan.FromMinutes(GlobalConstants.ENDPOINT_MASTER_CACHE_MINUTE));
                    return Ok(response);

                }
            }

        }


        [HttpGet("{Id}")]
        public async Task<ActionResult<GetRosterResponse>> Get(int Id, CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(new GetRosterRequest(Id), cancellationToken);
            return Ok(response);
        }

        [HttpPost]
        public async Task<ActionResult> Create(CreateRosterRequest request,
            CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(request, cancellationToken);
            _memoryCache.Remove($"API::{typeof(Roster).Name}");
            return Ok(response);
        }


        [HttpPut]
        public async Task<ActionResult> Update(UpdateRosterRequest request,
            CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(request, cancellationToken);
            _memoryCache.Remove($"API::{typeof(Roster).Name}");
            return Ok(response);
        }

        [HttpDelete]
        public async Task<ActionResult> Delete(DeleteRosterRequest request,
            CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(request, cancellationToken);
            _memoryCache.Remove($"API::{typeof(Roster).Name}");
            return Ok(response);
        }

    }
}
