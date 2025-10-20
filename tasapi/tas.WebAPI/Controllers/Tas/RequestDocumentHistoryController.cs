using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using tas.Application.Features.ClusterFeature.GetAllCluster;
using tas.Application.Features.RequestDocumentHistoryFeature.GetRequestDocumentHistory;
using tas.Application.Service;

namespace tas.WebAPI.Controllers.Tas
{

    [Route("api/tas/[controller]")]
    [ApiController]
    [RoleAuthorize]
    public class RequestDocumentHistoryController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<RequestDocumentHistoryController> _logger;

        public RequestDocumentHistoryController(IMediator mediator, ILogger<RequestDocumentHistoryController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }



        [HttpGet("{documentId}")]
        public async Task<ActionResult<List<GetRequestDocumentHistoryResponse>>> GetAll(int documentId, CancellationToken cancellationToken)
        {
           var response = await _mediator.Send(new GetRequestDocumentHistoryRequest(documentId), cancellationToken);
           return Ok(response);
        }

    }


}
