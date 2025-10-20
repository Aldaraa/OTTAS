using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using tas.Application.Features.DepartmentCostCodeFeature.AddDepartmentCostCode;
using tas.Application.Features.DepartmentCostCodeFeature.DeleteDepartmentCostCode;
using tas.Application.Features.DepartmentFeature.AddDepartmentManager;
using tas.Application.Repositories;
using tas.Application.Service;

namespace tas.WebAPI.Controllers.Tas
{



    [Route("api/tas/[controller]")]
    [ApiController]
    [RoleAuthorize]
    public class DepartmentCostCodeController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<DepartmentCostCodeController> _logger;
        private readonly HTTPUserRepository _HTTPUserRepository;

        private readonly CacheService _cacheService;

        //    private readonly IMemoryCache _memoryCache;

        public DepartmentCostCodeController(IMediator mediator, ILogger<DepartmentCostCodeController> logger, IMemoryCache memoryCache, HTTPUserRepository hTTPUserRepository, CacheService cacheService)
        {
            _mediator = mediator;
            _logger = logger;
            //  _memoryCache = memoryCache;
            _HTTPUserRepository = hTTPUserRepository;
            _cacheService = cacheService;
        }


        [HttpPost]
        public async Task<ActionResult> AddDepartmentCostcode(AddDepartmentCostCodeRequest request, CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(request, cancellationToken);
            return Ok(response);

        }

        [HttpDelete]
        public async Task<ActionResult> DeleteDepartmentCostcode(DeleteDepartmentCostCodeRequest request, CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(request, cancellationToken);
            return Ok(response);

        }

    }


}