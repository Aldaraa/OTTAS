using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using tas.Application.Features.RequestGroupFeature.CreateRequestGroup;
using tas.Application.Features.RequestGroupFeature.DeleteRequestGroup;
using tas.Application.Features.RequestGroupFeature.GetAllRequestGroup;
using tas.Application.Features.RequestGroupFeature.UpdateRequestGroup;
using tas.Application.Features.ShiftFeature.CreateShift;
using tas.Application.Features.ShiftFeature.DeleteShift;
using tas.Application.Features.ShiftFeature.GetAllShift;
using tas.Application.Features.ShiftFeature.UpdateShift;
using tas.Application.Service;

namespace tas.WebAPI.Controllers.Tas
{


    [Route("api/tas/[controller]")]
    [ApiController]
    [RoleAuthorize]
    public class RequestGroupController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<RequestGroupController> _logger;

        public RequestGroupController(IMediator mediator, ILogger<RequestGroupController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<List<GetAllRequestGroupResponse>>> GetAll(CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(new GetAllRequestGroupRequest(), cancellationToken);
            return Ok(response);
        }

        [HttpPost]
        public async Task<ActionResult> Create(CreateRequestGroupRequest request,
            CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(request, cancellationToken);
            return Ok(response);
        }


        [HttpPut]
        public async Task<ActionResult> Update(UpdateRequestGroupRequest request,
            CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(request, cancellationToken);
            return Ok(response);
        }

        [HttpDelete]
        public async Task<ActionResult> Delete(DeleteRequestGroupRequest request,
            CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(request, cancellationToken);
            return Ok(response);
        }

    }
}
