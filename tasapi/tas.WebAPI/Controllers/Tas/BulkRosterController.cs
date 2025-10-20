using MediatR;
using Microsoft.AspNetCore.Mvc;
using tas.Application.Features.EmployeeFeature.RosterExecuteEmployee;
using tas.Application.Features.RosterExecuteFeature.BulkRosterExecute;
using tas.Application.Features.RosterExecuteFeature.BulkRosterExecutePreview;
using tas.Application.Repositories;
using tas.Application.Service;

namespace tas.WebAPI.Controllers.Tas
{


    [Route("api/tas/[controller]")]
    [ApiController]
    [RoleAuthorize]
    public class BulkRosterController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<BulkRosterController> _logger;
        private readonly HTTPUserRepository _HTTPUserRepository;

        public BulkRosterController(IMediator mediator, ILogger<BulkRosterController> logger, HTTPUserRepository hTTPUserRepository)
        {
            _mediator = mediator;
            _logger = logger;
            _HTTPUserRepository = hTTPUserRepository;   
        }


        [HttpPost]
        public async Task<ActionResult> RosterExcecute(BulkRosterExecuteRequest request,
    CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(request, cancellationToken);
            _HTTPUserRepository.ClearAllEmployeeCache();
            return Ok(response);
        }


        [HttpPost("preview")]
        public async Task<ActionResult> RosterExcecutePreview(BulkRosterExecutePreviewRequest request,
    CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(request, cancellationToken);
            
            return Ok(response);
        }

    }

}
