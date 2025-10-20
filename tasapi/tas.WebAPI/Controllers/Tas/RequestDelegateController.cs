using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using tas.Application.Features.CampFeature.CreateCamp;
using tas.Application.Features.CampFeature.DeleteCamp;
using tas.Application.Features.CampFeature.GetAllCamp;
using tas.Application.Features.CampFeature.UpdateCamp;
using tas.Application.Features.RequestDelegateFeature.AllRequestDelegate;
using tas.Application.Features.RequestDelegateFeature.CreateRequestDelegate;
using tas.Application.Features.RequestDelegateFeature.DeleteRequestDelegate;
using tas.Application.Features.RequestDelegateFeature.UpdateRequestDelegate;
using tas.Application.Service;
using tas.Domain.Common;
using tas.Domain.Entities;

namespace tas.WebAPI.Controllers.Tas
{


    [Route("api/tas/[controller]")]
    [ApiController]
    [RoleAuthorize]
    public class RequestDelegateController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<RequestDelegateController> _logger;

        public RequestDelegateController(IMediator mediator, ILogger<RequestDelegateController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<List<AllRequestDelegateResponse>>> GetAll(int? fromEmployeeId, DateTime? startDate, DateTime? endDate, CancellationToken cancellationToken)
        {

            var response = await _mediator.Send(new AllRequestDelegateRequest(fromEmployeeId, startDate, endDate), cancellationToken);
            return Ok(response);



        }

        [HttpPost]
        public async Task<ActionResult> Create(CreateRequestDelegateRequest request,
            CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(request, cancellationToken);
            return Ok(response);
        }


        [HttpPut]
        public async Task<ActionResult> Update(UpdateRequestDelegateRequest request,
            CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(request, cancellationToken);
            return Ok(response);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id,
            CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(new DeleteRequestDelegateRequest(id), cancellationToken);
            return Ok(response);
        }
    }

}