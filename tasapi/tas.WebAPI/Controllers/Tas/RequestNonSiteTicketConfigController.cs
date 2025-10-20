using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using tas.Application.Features.NationalityFeature.GetAllNationality;
using tas.Application.Features.RequestNonSiteTicketConfigFeature.ExtractOptioRequestNonSiteTicket;
using tas.Application.Features.RequestNonSiteTicketConfigFeature.GetAllRequestNonSiteTicketConfig;
using tas.Application.Repositories;
using tas.Application.Service;
using tas.Domain.Common;
using tas.Domain.Entities;

namespace tas.WebAPI.Controllers.Tas
{

    [Route("api/tas/[controller]")]
    [ApiController]
    [RoleAuthorize]
    public class RequestNonSiteTicketConfigController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<RequestNonSiteTicketConfigController> _logger;
        private readonly IMemoryCache _memoryCache;

        private readonly HTTPUserRepository _hTTPUserRepository;


        public RequestNonSiteTicketConfigController(IMediator mediator, ILogger<RequestNonSiteTicketConfigController> logger, IMemoryCache memoryCache, HTTPUserRepository hTTPUserRepository)
        {
            _mediator = mediator;
            _logger = logger;
            _memoryCache = memoryCache;
            _hTTPUserRepository = hTTPUserRepository;
        }

        [HttpGet]
        public async Task<ActionResult<List<GetAllRequestNonSiteTicketConfigResponse>>> GetAll(int? active, CancellationToken cancellationToken)
        {
            var outData = new List<GetAllRequestNonSiteTicketConfigResponse>();
            string cacheEntityName = typeof(RequestNonSiteTicketConfig).Name;
            if (active == null)
            {
                if (_memoryCache.TryGetValue($"API::{cacheEntityName}", out outData))
                {
                    return Ok(outData);
                }
                else
                {

                    var response = await _mediator.Send(new GetAllRequestNonSiteTicketConfigRequest(null), cancellationToken);
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
                    var response = await _mediator.Send(new GetAllRequestNonSiteTicketConfigRequest(active), cancellationToken);
                    _memoryCache.Set($"API::{cacheEntityName}_{active}", response, TimeSpan.FromMinutes(GlobalConstants.ENDPOINT_MASTER_CACHE_MINUTE));
                    return Ok(response);

                }
            }


        }



        [HttpPost("extractoption")]
        public async Task<ActionResult<ExtractOptioRequestNonSiteTicketResponse>> ExtractData(ExtractOptioRequestNonSiteTicketRequest request, CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(request, cancellationToken);
            return Ok(response);


        }


    }
  }
