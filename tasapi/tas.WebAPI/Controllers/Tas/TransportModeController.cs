using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using tas.Application.Features.TransportModeFeature.CreateTransportMode;
using tas.Application.Features.TransportModeFeature.DeleteTransportMode;
using tas.Application.Features.TransportModeFeature.GetAllTransportMode;
using tas.Application.Features.TransportModeFeature.UpdateTransportMode;
using tas.Application.Service;

namespace tas.WebAPI.Controllers.Tas
{
    [Route("api/tas/[controller]")]
    [ApiController]
    [RoleAuthorize]
    public class TransportModeController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<TransportModeController> _logger;

        public TransportModeController(IMediator mediator, ILogger<TransportModeController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<List<GetAllTransportModeResponse>>> GetAll(int? active, CancellationToken cancellationToken)
        {
            if (active == null)
            {
                var response = await _mediator.Send(new GetAllTransportModeRequest(null), cancellationToken);
                return Ok(response);
            }
            else
            {
                var response = await _mediator.Send(new GetAllTransportModeRequest(active), cancellationToken);
                return Ok(response);

            }
        }

        [HttpPost]
        public async Task<ActionResult> Create(CreateTransportModeRequest request,
            CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(request, cancellationToken);
            return Ok(response);
        }


        [HttpPut]
        public async Task<ActionResult> Update(UpdateTransportModeRequest request,
            CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(request, cancellationToken);
            return Ok(response);
        }

        [HttpDelete]
        public async Task<ActionResult> Delete(DeleteTransportModeRequest request,
            CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(request, cancellationToken);
            return Ok(response);
        }

    }
}

