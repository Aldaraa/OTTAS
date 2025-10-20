using MediatR;
using Microsoft.AspNetCore.Mvc;
using tas.Application.Features.ColorFeature.CreateColor;
using tas.Application.Features.VisitEventFeature.CreateVisitEvent;
using tas.Application.Features.VisitEventFeature.DeleteVisitEvent;
using tas.Application.Features.VisitEventFeature.GetAllVisitEvent;
using tas.Application.Features.VisitEventFeature.GetVisitEvent;
using tas.Application.Features.VisitEventFeature.ReplaceProfile;
using tas.Application.Features.VisitEventFeature.ReplaceProfileMultiple;
using tas.Application.Features.VisitEventFeature.ReplaceProfileUndo;
using tas.Application.Features.VisitEventFeature.SetTransport;
using tas.Application.Features.VisitEventFeature.UpdateVisitEvent;
using tas.Application.Service;

namespace tas.WebAPI.Controllers.Tas
{

    [Route("api/tas/[controller]")]
    [ApiController]
    [RoleAuthorize]

    public class VisitEventController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<VisitEventController> _logger;

        public VisitEventController(IMediator mediator, ILogger<VisitEventController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult> GetAll(DateTime? startDate, DateTime? endDate, string? name, 
    CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(new GetAllVisitEventRequest(startDate, endDate, name), cancellationToken);
            return Ok(response);
        }


        [HttpGet("{Id}")]
        public async Task<ActionResult> Get(int Id,
    CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(new GetVisitEventRequest(Id), cancellationToken);
            return Ok(response);
        }




        [HttpPost]
        public async Task<ActionResult> Create(CreateVisitEventRequest request,
            CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(request, cancellationToken);
            return Ok(response);
        }


        [HttpPut]
        public async Task<ActionResult> Update(UpdateVisitEventRequest request,
    CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(request, cancellationToken);
            return Ok(response);
        }



        [HttpDelete]
        public async Task<ActionResult> Delete(DeleteVisitEventRequest request,
    CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(request, cancellationToken);
            return Ok(response);
        }

        [HttpPost("settransport")]
        public async Task<ActionResult> SetTransport(SetTransportRequest request,
    CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(request, cancellationToken);
            return Ok(response);
        }


        [HttpPost("replaceprofile")]
        public async Task<ActionResult> ReplaceProfile(ReplaceProfileRequest request,
CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(request, cancellationToken);
            return Ok(response);
        }

        [HttpPost("replaceprofileundo")]
        public async Task<ActionResult> ReplaceProfileUndo(ReplaceProfileUndoRequest request,
CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(request, cancellationToken);
            return Ok(response);
        }

        [HttpPost("replaceprofilemultiple")]
        public async Task<ActionResult> ReplaceProfileMultiple(ReplaceProfileMultipleRequest request,
CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(request, cancellationToken);
            return Ok(response);
        }


    }

}
