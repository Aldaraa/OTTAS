using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using tas.Application.Features.MailSmtpConfigFeature.CreateMailSmtpConfig;
using tas.Application.Features.MailSmtpConfigFeature.GetMailSmtpConfig;
using tas.Application.Service;

namespace tas.WebAPI.Controllers.Tas
{
    [Route("api/tas/[controller]")]
    [ApiController]
    [RoleAuthorize]

    public class MailSmtpConfigController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<CarrierController> _logger;

        public MailSmtpConfigController(IMediator mediator, ILogger<CarrierController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<GetMailSmtpConfigResponse>> Get(CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(new GetMailSmtpConfigRequest(), cancellationToken);
            return Ok(response);

        }



        [HttpPost("create")]
        public async Task<ActionResult> Create(CreateMailSmtpConfigRequest request, CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(request, cancellationToken);
            return Ok(response);

        }


        



    }
}
