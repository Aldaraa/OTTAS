using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using tas.Application.Features.CampFeature.GetAllCamp;
using tas.Application.Features.PositionFeature.GetAllRoomType;
using tas.Application.Features.RoomTypeFeature.CreateRoomType;
using tas.Application.Features.RoomTypeFeature.DeleteRoomType;
using tas.Application.Features.RoomTypeFeature.GetAllRoomType;
using tas.Application.Features.RoomTypeFeature.UpdateRoomType;
using tas.Application.Service;
using tas.Domain.Common;
using tas.Domain.Entities;

namespace tas.WebAPI.Controllers.Tas
{
    [Route("api/tas/[controller]")]
    [ApiController]
    [RoleAuthorize]
    public class RoomTypeController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<RoomTypeController> _logger;
        private readonly IMemoryCache _memoryCache;

        public RoomTypeController(IMediator mediator, ILogger<RoomTypeController> logger, IMemoryCache memoryCache)
        {
            _mediator = mediator;
            _logger = logger;
            _memoryCache = memoryCache;
        }

        [HttpGet]
        public async Task<ActionResult<List<GetAllRoomTypeResponse>>> GetAll(int? active, int? campId, CancellationToken cancellationToken)
        {
            var outData = new List<GetAllRoomTypeResponse>();
            string cacheEntityName = typeof(RoomType).Name;
            if (active == null)
            {
                var response = await _mediator.Send(new GetAllRoomTypeRequest(active, campId), cancellationToken);
                return Ok(response);
            }
            else
            {
                if (_memoryCache.TryGetValue($"API::{cacheEntityName}_{active}_{campId}", out outData))
                {
                    return Ok(outData);
                }
                else
                {
                    var response = await _mediator.Send(new GetAllRoomTypeRequest(active, campId), cancellationToken);
                    _memoryCache.Set($"API::{cacheEntityName}_{active}_{campId}", response, TimeSpan.FromMinutes(GlobalConstants.ENDPOINT_MASTER_CACHE_MINUTE));
                    return Ok(response);

                }
            }




        }

        [HttpPost]
        public async Task<ActionResult> Create(CreateRoomTypeRequest request,
            CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(request, cancellationToken);
            _memoryCache.Remove($"API::{typeof(RoomType).Name}_1");
            _memoryCache.Remove($"API::{typeof(RoomType).Name}_0");
            _memoryCache.Remove($"API::{typeof(RoomType).Name}");
            return Ok(response);
        }


        [HttpPut]
        public async Task<ActionResult> Update(UpdateRoomTypeRequest request,
            CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(request, cancellationToken);
            _memoryCache.Remove($"API::{typeof(RoomType).Name}_1");
            _memoryCache.Remove($"API::{typeof(RoomType).Name}_0");
            _memoryCache.Remove($"API::{typeof(RoomType).Name}");
            return Ok(response);
        }

        [HttpDelete]
        public async Task<ActionResult> Delete(DeleteRoomTypeRequest request,
            CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(request, cancellationToken);
            _memoryCache.Remove($"API::{typeof(RoomType).Name}_1");
            _memoryCache.Remove($"API::{typeof(RoomType).Name}_0");
            _memoryCache.Remove($"API::{typeof(RoomType).Name}");
            return Ok(response);
        }

    }
}
