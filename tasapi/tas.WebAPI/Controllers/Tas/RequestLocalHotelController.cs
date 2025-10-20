using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using tas.Application.Features.RequestLocalHotelFeature.CreateRequestLocalHotel;
using tas.Application.Features.RequestLocalHotelFeature.DeleteRequestLocalHotel;
using tas.Application.Features.RequestLocalHotelFeature.GetAllRequestLocalHotel;
using tas.Application.Features.RequestLocalHotelFeature.UpdateRequestLocalHotel;
using tas.Application.Service;

namespace tas.WebAPI.Controllers.Tas
{
  

    [Route("api/tas/[controller]")]
    [ApiController]
    [RoleAuthorize]

    public class RequestLocalHotelController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<RequestLocalHotelController> _logger;

        public RequestLocalHotelController(IMediator mediator, ILogger<RequestLocalHotelController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }


        [HttpGet]
        public async Task<ActionResult<List<GetAllRequestLocalHotelResponse>>> GetAll(int? active, CancellationToken cancellationToken)
        {
            if (active == null)
            {
                var response = await _mediator.Send(new GetAllRequestLocalHotelRequest(null), cancellationToken);
                return Ok(response);
            }
            else
            {
                var response = await _mediator.Send(new GetAllRequestLocalHotelRequest(active), cancellationToken);
                return Ok(response);

            }
        }

        [HttpPost]
        public async Task<ActionResult> Create(CreateRequestLocalHotelRequest request,
            CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(request, cancellationToken);
            return Ok(response);
        }


        [HttpPut]
        public async Task<ActionResult> Update(UpdateRequestLocalHotelRequest request,
            CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(request, cancellationToken);
            return Ok(response);
        }

        [HttpDelete]
        public async Task<ActionResult> Delete(DeleteRequestLocalHotelRequest request,
            CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(request, cancellationToken);
            return Ok(response);
        }

    }

}
