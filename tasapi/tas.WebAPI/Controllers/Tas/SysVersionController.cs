using DocumentFormat.OpenXml.Office2010.Excel;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using System.Collections.Generic;
using tas.Application.Features.SysTeamFeature.GetAllSysTeam;
using tas.Application.Features.SysTeamFeature.GetSysTeam;
using tas.Application.Features.SysVersionFeature.CreateSysVersion;
using tas.Application.Features.SysVersionFeature.GetSysVersion;
using tas.Application.Features.SysVersionHistoryFeature.GetSysVersionHistory;
using tas.Application.Features.SysVersionNoteFeature.GetSysVersionNote;
using tas.Application.Repositories;
using tas.Application.Service;
using tas.Domain.Common;

namespace tas.WebAPI.Controllers.Tas
{

    [Route("api/tas/[controller]")]
    [ApiController]
    [RoleAuthorize]
    public class SysVersionController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<SysVersionController> _logger;
        private readonly CacheService _cacheService;
        private readonly HTTPUserRepository _hTTPUserRepository;

        public SysVersionController(IMediator mediator, ILogger<SysVersionController> logger, HTTPUserRepository hTTPUserRepository, CacheService cacheService)
        {
            _mediator = mediator;
            _logger = logger;
            _hTTPUserRepository = hTTPUserRepository;
            _cacheService = cacheService;
        }


        [HttpGet]
        public async Task<ActionResult<GetSysVersionResponse>> GetSysVersion(CancellationToken cancellationToken)
        {
            var cacheKey = $"GetSysVersion";

            if (!_cacheService.TryGetValue(cacheKey, out GetSysVersionResponse response))
            {
               response = await _mediator.Send(new GetSysVersionRequest(), cancellationToken);
                _cacheService.Set(cacheKey, response, GlobalConstants.ENDPOINT_MASTER_STATIC_MINUTE);
            }

            return Ok(response);
        }

        [HttpGet("releasenote/{Id}")]
        public async Task<ActionResult<GetSysVersionNoteResponse>> Get(int Id, CancellationToken cancellationToken)
        {

            var cacheKey = $"GetSysVersionNote_{Id}";

            if (!_cacheService.TryGetValue(cacheKey, out GetSysVersionNoteResponse response))
            {
                response = await _mediator.Send(new GetSysVersionNoteRequest(Id), cancellationToken);


                _cacheService.Set(cacheKey, response, GlobalConstants.ENDPOINT_MASTER_STATIC_MINUTE);
            }

            return Ok(response);
        }

        [HttpGet("versionhistory")]
        public async Task<ActionResult<List<GetSysVersionHistoryResponse>>> GetHistory( CancellationToken cancellationToken)
        {
            var cacheKey = "GetSysVersionHistory";
            if (!_cacheService.TryGetValue(cacheKey, out List<GetSysVersionHistoryResponse> response))
            {
               response = await _mediator.Send(new GetSysVersionHistoryRequest(), cancellationToken);
                _cacheService.Set(cacheKey, response, GlobalConstants.ENDPOINT_MASTER_STATIC_MINUTE);
            }

            return Ok(response);
        }

        [HttpPost]
        public async Task<ActionResult<GetSysVersionNoteResponse>> CreateVersion(CreateSysVersionRequest request, CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(request, cancellationToken);
            _cacheService.RemoveByPrefix("GetSysVersionNote_");
            _cacheService.Remove("GetSysVersionHistory");
            _cacheService.Remove("GetSysVersion");

            return Ok(response);
        }

    }

}
