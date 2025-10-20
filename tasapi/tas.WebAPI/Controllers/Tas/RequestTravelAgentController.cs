using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using tas.Application.Features.EmployerFeature.GetAllEmployer;
using tas.Application.Features.RequestTravelAgentFeature.CreateRequestTravelAgent;
using tas.Application.Features.RequestTravelAgentFeature.DeleteRequestTravelAgent;
using tas.Application.Features.RequestTravelAgentFeature.GetAllRequestTravelAgent;
using tas.Application.Features.RequestTravelAgentFeature.UpdateRequestTravelAgent;
using tas.Application.Repositories;
using tas.Application.Service;
using tas.Domain.Common;
using tas.Domain.Entities;

namespace tas.WebAPI.Controllers.Tas
{


    [Route("api/tas/[controller]")]
    [ApiController]
    [RoleAuthorize]
    public class RequestTravelAgentController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<RequestTravelAgentController> _logger;
        private readonly CacheService _cacheService;
        private readonly HTTPUserRepository _userRepository;

        public RequestTravelAgentController(IMediator mediator, ILogger<RequestTravelAgentController> logger, CacheService cacheService, HTTPUserRepository userRepository)
        {
            _mediator = mediator;
            _logger = logger;
            _cacheService = cacheService;
            _userRepository = userRepository;
        }

        [HttpGet]
        public async Task<ActionResult<List<GetAllRequestTravelAgentResponse>>> GetAll(int? active, CancellationToken cancellationToken)
        {


            var outData = new List<GetAllRequestTravelAgentResponse>();
            string cacheEntityName = typeof(RequestTravelAgent).Name;
            if (active == null)
            {
                if (_cacheService.TryGetValue($"API::{cacheEntityName}", out outData))
                {
                    return Ok(outData);
                }
                else
                {

                    var response = await _mediator.Send(new GetAllRequestTravelAgentRequest(null), cancellationToken);
                    _cacheService.Set($"API::{cacheEntityName}", response, TimeSpan.FromMinutes(GlobalConstants.ENDPOINT_MASTER_CACHE_MINUTE));
                    return Ok(response);

                }
            }
            else
            {
                if (_cacheService.TryGetValue($"API::{cacheEntityName}_{active}", out outData))
                {
                    return Ok(outData);
                }
                else
                {
                    var response = await _mediator.Send(new GetAllRequestTravelAgentRequest(active), cancellationToken);
                    _cacheService.Set($"API::{cacheEntityName}_{active}", response, TimeSpan.FromMinutes(GlobalConstants.ENDPOINT_MASTER_CACHE_MINUTE));
                    return Ok(response);

                }
            }




        }

        [HttpPost]
        public async Task<ActionResult> Create(CreateRequestTravelAgentRequest request,
            CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(request, cancellationToken);
            _userRepository.ClearAllMasterCache<RequestTravelAgent>();
            return Ok(response);
        }


        [HttpPut]
        public async Task<ActionResult> Update(UpdateRequestTravelAgentRequest request,
            CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(request, cancellationToken);
            _userRepository.ClearAllMasterCache<RequestTravelAgent>();
            return Ok(response);
        }

        [HttpDelete]
        public async Task<ActionResult> Delete(DeleteRequestTravelAgentRequest request,
            CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(request, cancellationToken);
            _userRepository.ClearAllMasterCache<RequestTravelAgent>();
            return Ok(response);
        }

    }

}
