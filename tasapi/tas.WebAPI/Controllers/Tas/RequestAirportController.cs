using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using tas.Application.Features.CostCodeFeature.CreateCostCode;
using tas.Application.Features.CostCodeFeature.DeleteCostCode;
using tas.Application.Features.CostCodeFeature.GetAllCostCode;
using tas.Application.Features.CostCodeFeature.UpdateCostCode;
using tas.Application.Features.LocationFeature.GetAllLocation;
using tas.Application.Features.RequestAirportFeature.CreateRequestAirport;
using tas.Application.Features.RequestAirportFeature.DeleteRequestAirport;
using tas.Application.Features.RequestAirportFeature.GetAllRequestAirport;
using tas.Application.Features.RequestAirportFeature.SearchRequestAirport;
using tas.Application.Features.RequestAirportFeature.UpdateRequestAirport;
using tas.Application.Repositories;
using tas.Application.Service;
using tas.Domain.Common;
using tas.Domain.Entities;
using static Microsoft.Extensions.Logging.EventSource.LoggingEventSource;

namespace tas.WebAPI.Controllers.Tas
{


    [Route("api/tas/[controller]")]
    [ApiController]
    [RoleAuthorize]
    public class RequestAirportController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<RequestAirportController> _logger;
        private readonly CacheService _memoryCache;
        private readonly HTTPUserRepository _HTTPUserRepository;

        public RequestAirportController(IMediator mediator, ILogger<RequestAirportController> logger, CacheService memoryCache, HTTPUserRepository hTTPUserRepository)
        {
            _mediator = mediator;
            _logger = logger;
            _memoryCache = memoryCache;
            _HTTPUserRepository = hTTPUserRepository;
        }

        [HttpGet]
        public async Task<ActionResult<List<GetAllRequestAirportResponse>>> GetAll(int? active, CancellationToken cancellationToken)
        {

            var outData = new List<GetAllRequestAirportResponse>();
            string cacheEntityName = typeof(RequestAirport).Name;
            if (active == null)
            {
                if (_memoryCache.TryGetValue($"API::{cacheEntityName}", out outData))
                {
                    return Ok(outData);
                }
                else
                {

                    var response = await _mediator.Send(new GetAllRequestAirportRequest(null), cancellationToken);
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
                    var response = await _mediator.Send(new GetAllRequestAirportRequest(active), cancellationToken);
                    _memoryCache.Set($"API::{cacheEntityName}_{active}", response, TimeSpan.FromMinutes(GlobalConstants.ENDPOINT_MASTER_CACHE_MINUTE));
                    return Ok(response);

                }
            }


            //var response = await _mediator.Send(new GetAllRequestAirportRequest(active), cancellationToken);
            //return Ok(response);
        }

        [HttpGet("search/{keyword}")]
        public async Task<ActionResult<List<SearchRequestAirportResponse>>> SearchData(string keyword, CancellationToken cancellationToken)
        {

            var response = await _mediator.Send(new SearchRequestAirportRequest(keyword), cancellationToken);
            return Ok(response);
        }

        [HttpPost]
        public async Task<ActionResult> Create(CreateRequestAirportRequest request,
            CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(request, cancellationToken);
            _HTTPUserRepository.ClearAllMasterCache<RequestAirport>();
            return Ok(response);
        }


        [HttpPut]
        public async Task<ActionResult> Update(UpdateRequestAirportRequest request,
            CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(request, cancellationToken);
            _HTTPUserRepository.ClearAllMasterCache<RequestAirport>();

            return Ok(response);
        }

        [HttpDelete]
        public async Task<ActionResult> Delete(DeleteRequestAirportRequest request,
            CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(request, cancellationToken);
            _HTTPUserRepository.ClearAllMasterCache<RequestAirport>();
            return Ok(response);
        }
    }

}
