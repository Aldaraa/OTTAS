using MediatR;
using Microsoft.AspNetCore.Mvc;
using tas.Application.Features.GroupMasterFeature.CreateGroupMaster;
using tas.Application.Features.GroupMasterFeature.DeleteGroupMaster;
using tas.Application.Features.GroupMasterFeature.GetAllGroupMaster;
using tas.Application.Features.GroupMasterFeature.GetProfileGroupMaster;
using tas.Application.Features.GroupMasterFeature.SetSeqGroupMaster;
using tas.Application.Features.GroupMasterFeature.UpdateGroupMaster;
using tas.Application.Features.ShiftFeature.GetAllShift;
using tas.Application.Repositories;
using tas.Application.Service;
using tas.Domain.Common;
using tas.Domain.Entities;

namespace tas.WebAPI.Controllers.Tas
{
    [Route("api/tas/[controller]")]
    [ApiController]
    [RoleAuthorize]
    public class GroupMasterController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<GroupMasterController> _logger;
        private readonly CacheService _memoryCache;
        private readonly HTTPUserRepository _hTTPUserRepository;

        public GroupMasterController(IMediator mediator, ILogger<GroupMasterController> logger, CacheService cacheService, HTTPUserRepository hTTPUserRepository)
        {
            _mediator = mediator;
            _logger = logger;
            _memoryCache = cacheService;
            _hTTPUserRepository = hTTPUserRepository;
        }

        [HttpGet]
        public async Task<ActionResult<List<GetAllGroupMasterResponse>>> GetAll(int? active, CancellationToken cancellationToken)
        {
            var outData = new List<GetAllGroupMasterResponse>();
            string cacheEntityName = typeof(GroupMaster).Name;
            if (active == null)
            {
                if (_memoryCache.TryGetValue($"API::{cacheEntityName}", out outData))
                {
                    return Ok(outData);
                }
                else
                {

                    var response = await _mediator.Send(new GetAllGroupMasterRequest(null), cancellationToken);
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
                    var response = await _mediator.Send(new GetAllGroupMasterRequest(active), cancellationToken);
                    _memoryCache.Set($"API::{cacheEntityName}_{active}", response, TimeSpan.FromMinutes(GlobalConstants.ENDPOINT_MASTER_CACHE_MINUTE));
                    return Ok(response);

                }
            }






            //if (active == null)
            //{
            //    var response = await _mediator.Send(new GetAllGroupMasterRequest(null), cancellationToken);
            //    return Ok(response);
            //}
            //else
            //{
            //    var response = await _mediator.Send(new GetAllGroupMasterRequest(active), cancellationToken);
            //    return Ok(response);

            //}
        }


        [HttpGet("profiledata")]
        public async Task<ActionResult<List<GetAllGroupMasterResponse>>> GetProfile(CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(new GetProfileGroupMasterRequest(), cancellationToken);
            return Ok(response);
        }

        [HttpPost]
        public async Task<ActionResult> Create(CreateGroupMasterRequest request,
            CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(request, cancellationToken);
            _hTTPUserRepository.ClearAllMasterCache<GroupMaster>();
            string cacheEntityName = "GroupMasterAudit";
            var cacheKey = $"API::{cacheEntityName}";
            _memoryCache.Remove(cacheKey);
            return Ok(response);
        }


        [HttpPut]
        public async Task<ActionResult> Update(UpdateGroupMasterRequest request,
            CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(request, cancellationToken);
            _hTTPUserRepository.ClearAllMasterCache<GroupMaster>();
            string cacheEntityName = "GroupMasterAudit";
            var cacheKey = $"API::{cacheEntityName}";
            _memoryCache.Remove(cacheKey);
            return Ok(response);
        }


        [HttpPost("changeorderby")]
        public async Task<ActionResult> ChangeOrderBy(SetSeqGroupMasterRequest request,
            CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(request, cancellationToken);
            _hTTPUserRepository.ClearAllMasterCache<GroupMaster>();

            string cacheEntityName = "GroupMasterAudit";
            var cacheKey = $"API::{cacheEntityName}";
            _memoryCache.Remove(cacheKey);
            return Ok(response);
        }

        [HttpDelete]
        public async Task<ActionResult> Delete(DeleteGroupMasterRequest request,
            CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(request, cancellationToken);
            _hTTPUserRepository.ClearAllMasterCache<GroupMaster>();
            string cacheEntityName = "GroupMasterAudit";
            var cacheKey = $"API::{cacheEntityName}";
            _memoryCache.Remove(cacheKey);
            return Ok(response);
        }

    }
}
