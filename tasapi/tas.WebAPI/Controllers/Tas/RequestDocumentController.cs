using DocumentFormat.OpenXml.Office2010.Excel;
using DocumentFormat.OpenXml.Office2010.Word;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using System.Collections.Generic;
using System.Reflection;
using tas.Application.Features.CampFeature.GetAllCamp;
using tas.Application.Features.DashboardFeature.EmployeeDashboard;
using tas.Application.Features.DashboardFeature.TransportDashboard;
using tas.Application.Features.MailSmtpConfigFeature.SendMailRequestDocument;
using tas.Application.Features.RequestDocumentAttachmentFeature.GetRequestDocumentAttachment;
using tas.Application.Features.RequestDocumentFeature.ApproveRequestDocument;
using tas.Application.Features.RequestDocumentFeature.CancelRequestDocument;
using tas.Application.Features.RequestDocumentFeature.ChangeLineManagerRequestDocument;
using tas.Application.Features.RequestDocumentFeature.CheckDuplicateRequestDocument;
using tas.Application.Features.RequestDocumentFeature.CreateRequestDocumentNonSiteTravel;
using tas.Application.Features.RequestDocumentFeature.DeclineRequestDocument;
using tas.Application.Features.RequestDocumentFeature.ExistingBookingRequestDocument;
using tas.Application.Features.RequestDocumentFeature.GenerateCompletedDeclinedChange;
using tas.Application.Features.RequestDocumentFeature.GenerateDescriptionTest;
using tas.Application.Features.RequestDocumentFeature.GetDocumentList;
using tas.Application.Features.RequestDocumentFeature.GetDocumentListCancelled;
using tas.Application.Features.RequestDocumentFeature.GetDocumentListInpersonate;
using tas.Application.Features.RequestDocumentFeature.GetNonSiteTravelGroup;
using tas.Application.Features.RequestDocumentFeature.GetNonSiteTravelMaster;
using tas.Application.Features.RequestDocumentFeature.GetRequestDocumentMyInfo;
using tas.Application.Features.RequestDocumentFeature.GetRequestDocumentNonSiteTravel;
using tas.Application.Features.RequestDocumentFeature.RemoveCancelRequestDocument;
using tas.Application.Features.RequestDocumentFeature.UpdateRequestDocumentNonSiteTravelData;
using tas.Application.Features.RequestDocumentFeature.UpdateRequestDocumentNonSiteTravelEmployee;
using tas.Application.Repositories;
using tas.Application.Service;
using tas.Domain.Common;

namespace tas.WebAPI.Controllers.Tas
{


    [Route("api/tas/[controller]")]
    [ApiController]
    [RoleAuthorize]
    public class RequestDocumentController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<RequestDocumentController> _logger;
        private readonly CacheService _cacheService;
        private readonly HTTPUserRepository _httpUserRepository;
        public RequestDocumentController(IMediator mediator, ILogger<RequestDocumentController> logger,  HTTPUserRepository httpUserRepository, CacheService cacheService)
        {
            _mediator = mediator;
            _logger = logger;
            _httpUserRepository = httpUserRepository;
            _cacheService = cacheService;
        }


        [HttpGet("master")]
        public async Task<ActionResult<GetNonSiteTravelMasterResponse>> GetMaster(CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(new GetNonSiteTravelMasterRequest(), cancellationToken);
            return Ok(response);
        }


        [HttpPost("documentlist")]
        public async Task<ActionResult<GetDocumentListResponse>> DocumentList(GetDocumentListRequest request, CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(request, cancellationToken);
            return Ok(response);
        }

        [HttpPost("documentlist/cancelled")]
        public async Task<ActionResult> DocumentListCancelled(GetDocumentListCancelledRequest request, CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(request, cancellationToken);
            return Ok(response);
        }

        [HttpGet("documentlistInpersonate/{employeeId}")]
        public async Task<ActionResult<List<GetDocumentListInpersonateResponse>>> DocumentListInpersonate(int employeeId, CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(new GetDocumentListInpersonateRequest(employeeId), cancellationToken);
            return Ok(response);
        }


        [HttpPut("cancel")]
        public async Task<ActionResult> CancelDocument(CancelRequestDocumentRequest request, CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(request, cancellationToken);
            await _mediator.Send(new SendMailRequestDocumentRequest(request.Id), cancellationToken);
            return Ok(response);
        }



        [HttpDelete("remove")]
        public async Task<ActionResult> RemoveDocument(RemoveCancelRequestDocumentRequest request, CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(request, cancellationToken);
            return Ok(response);
        }


        [HttpPut("approve")]
        public async Task<ActionResult> ApproveDocument(ApproveRequestDocumentRequest request, CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(request, cancellationToken);
            await _mediator.Send(new SendMailRequestDocumentRequest(request.Id), cancellationToken);
            
    

            return Ok(response);

        }


        [HttpPut("changelinemanager")]
        public async Task<ActionResult> ChangeLineManagerDocument(ChangeLineManagerRequestDocumentRequest request, CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(request, cancellationToken);
            await _mediator.Send(new SendMailRequestDocumentRequest(request.Id), cancellationToken);
            return Ok(response);

        }


        [HttpPut("decline")]
        public async Task<ActionResult> DeclineDocument(DeclineRequestDocumentRequest request, CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(request, cancellationToken);
            await _mediator.Send(new SendMailRequestDocumentRequest(request.Id), cancellationToken);
            return Ok(response);
        }

        [HttpGet("checkduplicate/{documenttype}/{employeeId}")]
        public async Task<ActionResult<List<CheckDuplicateRequestDocumentResponse>>> CheckDuplcate(string documenttype, int employeeId, string? documentTag, DateTime? startdate, DateTime? enddate, CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(new CheckDuplicateRequestDocumentRequest(documenttype, employeeId, documentTag, startdate, enddate), cancellationToken);
            return Ok(response);
        }


        [HttpGet("existingbooking/{EmployeeId}")]

        public async Task<ActionResult<ExistingBookingRequestDocumentResponse>> ExistingBooking(int EmployeeId, CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(new ExistingBookingRequestDocumentRequest(EmployeeId), cancellationToken);
            return Ok(response);
        }



        [HttpGet("nonsitetravel/groups")]

        public async Task<ActionResult<List<GetNonSiteTravelGroupResponse>>> GetNonSiteTravelGroupRequest(CancellationToken cancellationToken)
        {


            var response = await _mediator.Send(new GetNonSiteTravelGroupRequest(), cancellationToken);
            return Ok(response);
        }


        [HttpGet("myinfo")]

        public async Task<ActionResult<GetRequestDocumentMyInfoResponse>> GetRequestDocumentMyInfo(CancellationToken cancellationToken)
        {
            var userId = _httpUserRepository.LogCurrentUser()?.Id;
            var cacheKey = $"GetRequestDocumentMyInfo_{userId}";

            if (!_cacheService.TryGetValue(cacheKey, out GetRequestDocumentMyInfoResponse response))
            {
                response = await _mediator.Send(new GetRequestDocumentMyInfoRequest(), cancellationToken);
                _cacheService.Set(cacheKey, response, GlobalConstants.ENDPOINT_MASTER_STATIC_MINUTE);
            }

            return Ok(response);

            //var username = _httpUserRepository.LogCurrentUser()?.UserName;
            //var cacheKey = $"GetRequestDocumentMyInfo_{username}";

            //if (!_memoryCache.TryGetValue(cacheKey, out GetRequestDocumentMyInfoResponse result))
            //{
            //    var response = await _mediator.Send(new GetRequestDocumentMyInfoRequest(), cancellationToken);
            //    result = response;

            //    _memoryCache.Set(cacheKey, result, TimeSpan.FromMinutes(GlobalConstants.ENDPOINT_MASTER_CACHE_MINUTE));
            //}

            //return Ok(result);

            //var response = await _mediator.Send(new GetRequestDocumentMyInfoRequest(), cancellationToken);
            //return Ok(response);
        }
        [HttpGet("testnotification/{Id}")]

        public async Task<ActionResult> TestNotification(int Id, CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(new SendMailRequestDocumentRequest(Id), cancellationToken);
            return Ok(response);
        }


        [HttpPost("GenerateCompletedDeclinedChange")]

        // TODO: This endpoint is for testing purposes. Only available for developer use.

        public async Task<ActionResult> GenerateCompletedDeclinedChange(GenerateCompletedDeclinedChangeRequest request, CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(request, cancellationToken);
            return Ok(response);
        }


        [HttpPost("GenerateDescriptionTest")]

        // TODO: This endpoint is for testing purposes. Only available for developer use.

        public async Task<ActionResult> GenerateDescriptionTest(GenerateDescriptionTestRequest request, CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(request, cancellationToken);
            return Ok(response);
        }



    }

}
