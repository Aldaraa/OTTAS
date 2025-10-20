using Application.Features.ReportJobFeature.CreateJob;
using Application.Features.ReportTemplateFeature.GetAllReportTemplate;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace tas.ReportAPI.Controllers
{


    [Route("rapi/tasreport/[controller]")]
    [ApiController]
    public class MailController : ControllerBase
    {
        private readonly IMediator _mediator;


        public MailController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<ActionResult> SendMail(SendMailRequest request, CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(request, cancellationToken);
            return Ok(response);
        }


        [HttpPost("newjob")]
        public async Task<ActionResult> CreateJob(CreateJobRequest request, CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(request, cancellationToken);
            return Ok(response);
        }
    }
}
