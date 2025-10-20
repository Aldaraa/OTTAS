using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using tas.Application.Features.CarrierFeature.CreateCarrier;
using tas.Application.Features.CarrierFeature.DeleteCarrier;
using tas.Application.Features.CarrierFeature.GetAllCarrier;
using tas.Application.Features.CarrierFeature.UpdateCarrier;
using tas.Application.Service;

namespace tas.WebAPI.Controllers.Tas
{
    [Route("api/tas/[controller]")]
    [ApiController]
    [RoleAuthorize]
    public class CarrierController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<CarrierController> _logger;

        public CarrierController(IMediator mediator, ILogger<CarrierController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<List<GetAllCarrierResponse>>> GetAll(int? active, CancellationToken cancellationToken)
        {
            if (active == null)
            {
                var response = await _mediator.Send(new GetAllCarrierRequest(null), cancellationToken);
                return Ok(response);
            }
            else
            {
                var response = await _mediator.Send(new GetAllCarrierRequest(active), cancellationToken);
                return Ok(response);

            }
        }

        [HttpPost]
        public async Task<ActionResult> Create(CreateCarrierRequest request,
            CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(request, cancellationToken);
            return Ok(response);
        }


        [HttpPut]
        public async Task<ActionResult> Update(UpdateCarrierRequest request,
            CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(request, cancellationToken);
            return Ok(response);
        }

        [HttpDelete]
        public async Task<ActionResult> Delete(DeleteCarrierRequest request,
            CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(request, cancellationToken);
            return Ok(response);
        }

    }
}
