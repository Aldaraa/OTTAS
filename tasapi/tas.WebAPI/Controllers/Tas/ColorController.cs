using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using tas.Application.Features.ColorFeature.CreateColor;
using tas.Application.Features.ColorFeature.DeleteColor;
using tas.Application.Features.ColorFeature.GetAllColor;
using tas.Application.Features.ColorFeature.UpdateColor;
using tas.Application.Service;

namespace tas.WebAPI.Controllers.Tas
{
    [Route("api/tas/[controller]")]
    [ApiController]
    [RoleAuthorize]
    
    public class ColorController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<ColorController> _logger;

        public ColorController(IMediator mediator, ILogger<ColorController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }



        [HttpGet]
        public async Task<ActionResult<List<GetAllColorResponse>>> GetAll(int? active, CancellationToken cancellationToken)
        {
            if (active == null)
            {
                var response = await _mediator.Send(new GetAllColorRequest(null), cancellationToken);
                return Ok(response);
            }
            else
            {
                var response = await _mediator.Send(new GetAllColorRequest(active), cancellationToken);
                return Ok(response);

            }
        }

        [HttpPost]
        public async Task<ActionResult> Create(CreateColorRequest request,
            CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(request, cancellationToken);
            return Ok(response);
        }


        [HttpPut]
        public async Task<ActionResult> Update(UpdateColorRequest request,
            CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(request, cancellationToken);
            return Ok(response);
        }

        [HttpDelete]
        public async Task<ActionResult> Delete(DeleteColorRequest request,
            CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(request, cancellationToken);
            return Ok(response);
        }

    }
}