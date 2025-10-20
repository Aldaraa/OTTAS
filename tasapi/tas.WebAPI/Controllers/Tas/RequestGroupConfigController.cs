using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using tas.Application.Features.RequestDocumentFeature.GetRequestDocumentMyInfo;
using tas.Application.Features.RequestGroupConfigFeature.GetRequestDocumentType;
using tas.Application.Features.RequestGroupConfigFeature.RequestDocumentGroup;
using tas.Application.Features.RequestGroupConfigFeature.RequestDocumentGroupAdd;
using tas.Application.Features.RequestGroupConfigFeature.RequestDocumentGroupById;
using tas.Application.Features.RequestGroupConfigFeature.RequestDocumentGroupByType;
using tas.Application.Features.RequestGroupConfigFeature.RequestDocumentGroupEmpLines;
using tas.Application.Features.RequestGroupConfigFeature.RequestDocumentGroupOrder;
using tas.Application.Features.RequestGroupConfigFeature.RequestDocumentGroupRemove;
using tas.Application.Features.RequestGroupConfigFeature.RequestDocumentGroupUpdate;
using tas.Application.Features.RequestGroupConfigFeature.RequestDocumentRoute;
using tas.Application.Features.RequestGroupFeature.GetAllRequestGroup;
using tas.Application.Repositories;
using tas.Application.Service;
using tas.Domain.Common;

namespace tas.WebAPI.Controllers.Tas
{


    [Route("api/tas/[controller]")]
    [ApiController]
    [RoleAuthorize]
    public class RequestGroupConfigController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<RequestGroupConfigController> _logger;
        private readonly CacheService _cacheService;

        public RequestGroupConfigController(IMediator mediator, ILogger<RequestGroupConfigController> logger, CacheService cacheService)
        {
            _mediator = mediator;
            _logger = logger;
            _cacheService = cacheService;
        }

        [HttpGet("documenttypes")]
        public async Task<ActionResult<List<GetRequestDocumentTypeResponse>>> GetDocumentTypes(CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(new GetRequestDocumentTypeRequest(), cancellationToken);
            return Ok(response);
        }


        [HttpGet("documentapproval/{document}")]
        public async Task<ActionResult<List<GetRequestDocumentTypeResponse>>> GetDocumentTypes(string document, CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(new RequestDocumentGroupRequest(document), cancellationToken);
            return Ok(response);
        }


        [HttpPost("documentapproval")]
        public async Task<ActionResult> AddApproval(RequestDocumentGroupAddRequest request, CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(request, cancellationToken);
            return Ok(response);
        }

        [HttpPut("documentapproval")]
        public async Task<ActionResult> UpdateApproval(RequestDocumentGroupUpdateRequest request, CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(request, cancellationToken);
            return Ok(response);
        }


        [HttpPut("documentapprovalorder")]
        public async Task<ActionResult> OrderApproval(RequestDocumentGroupOrderRequest request, CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(request, cancellationToken);
            return Ok(response);
        }


        [HttpDelete("{Id}")]
        public async Task<ActionResult> RemoveApproval(int Id, CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(new RequestDocumentGroupRemoveRequest(Id), cancellationToken);
            return Ok(response);
        }

        [HttpGet("groupandmembers/{documentId}")]
        public async Task<ActionResult> getDocumentGroupsAndMembersById(int documentId, CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(new RequestDocumentGroupByIdRequest(documentId), cancellationToken);
            return Ok(response);
        }



        [HttpGet("groupandmembers/bytype/{documentType}")]
        public async Task<ActionResult> getDocumentGroupsAndMembersByType(string documentType,int? empId, CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(new RequestDocumentGroupByTypeRequest(documentType, empId), cancellationToken);
            return Ok(response);
        }





        //RequestDocumentRouteRequest

        [HttpGet("documentroute/{documentId}")]
        public async Task<ActionResult> GetRequestDocumentRoute(int documentId, CancellationToken cancellationToken)
        {

            var  response = await _mediator.Send(new RequestDocumentRouteRequest(documentId), cancellationToken);

            return Ok(response);






            //var cacheKey = $"documentroute_{documentId}";

            //if (!_cacheService.TryGetValue(cacheKey, out List<RequestDocumentRouteResponse> response))
            //{
            //    response = await _mediator.Send(new RequestDocumentRouteRequest(documentId), cancellationToken);
            //    _cacheService.Set(cacheKey, response, GlobalConstants.ENDPOINT_MASTER_STATIC_MINUTE);
            //}

            //return Ok(response);



        }

        [HttpGet("employee/linemanagers/{empId}")]
        public async Task<ActionResult> getDocumentGroupsAndMembersByType(int empId, CancellationToken cancellationToken)   
        {
            var response = await _mediator.Send(new RequestDocumentGroupEmpLinesRequest(empId), cancellationToken);
            return Ok(response);
        }


    }

}