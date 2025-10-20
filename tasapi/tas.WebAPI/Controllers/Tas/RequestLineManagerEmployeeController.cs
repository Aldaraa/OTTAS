using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using tas.Application.Features.RequestGroupEmployeeFeature.GetAllRequestGroupEmployees;
using tas.Application.Features.RequestLineManagerEmployeeFeature.CreateRequestLineManagerEmployee;
using tas.Application.Features.RequestLineManagerEmployeeFeature.GetRequestLineManagerEmployee;
using tas.Application.Features.RequestLineManagerEmployeeFeature.RemoveRequestLineManagerEmployee;
using tas.Application.Service;

namespace tas.WebAPI.Controllers.Tas
{
    [Route("api/tas/[controller]")]
    [ApiController]
    [RoleAuthorize]
    public class RequestLineManagerEmployeeController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<RequestLineManagerEmployeeController> _logger;
        private readonly IMemoryCache _memoryCache;

        public RequestLineManagerEmployeeController(IMediator mediator, ILogger<RequestLineManagerEmployeeController> logger, IMemoryCache memoryCache)
        {
            _mediator = mediator;
            _logger = logger;
            _memoryCache = memoryCache;
        }

        [HttpGet]
        public async Task<ActionResult<List<GetRequestLineManagerEmployeeResponse>>> GetRequestLineManagerEmployee(CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(new GetRequestLineManagerEmployeeRequest(), cancellationToken);
            return Ok(response);
        }


        [HttpPost]
        public async Task<ActionResult> CreateRequestLineManagerEmployee(CreateRequestLineManagerEmployeeRequest request, CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(request, cancellationToken);
            return Ok(response);
        }



        [HttpDelete("{Id}")]
        public async Task<ActionResult> DeleteRequestLineManagerEmployee(int Id, CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(new RemoveRequestLineManagerEmployeeRequest(Id), cancellationToken);
            return Ok(response);
        }

    }


}
