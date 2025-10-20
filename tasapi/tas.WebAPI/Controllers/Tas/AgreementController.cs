using DocumentFormat.OpenXml.Office2010.Word;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using tas.Application.Features.AgreementFeature.CreateAgreement;
using tas.Application.Features.AgreementFeature.GetAgreement;
using tas.Application.Features.AuthenticationFeature.AgreementCheck;
using tas.Application.Features.CarrierFeature.GetAllCarrier;
using tas.Application.Features.RequestDocumentAttachmentFeature.GetRequestDocumentAttachment;
using tas.Application.Service;
using tas.Domain.Common;

namespace tas.WebAPI.Controllers.Tas
{
    [Route("api/tas/[controller]")]
    [ApiController]
    [RoleAuthorize]

    public class AgreementController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<CarrierController> _logger;
        private readonly CacheService _cacheService;


        public AgreementController(IMediator mediator, ILogger<CarrierController> logger, CacheService cacheService)
        {
            _mediator = mediator;
            _logger = logger;
            _cacheService = cacheService;
        }

        [HttpGet]
        public async Task<ActionResult<GetAgreementResponse>> Get(CancellationToken cancellationToken)
        {

            var cacheKey = $"GetAgreement";

            if (!_cacheService.TryGetValue(cacheKey, out GetAgreementResponse response))
            {
                response = await _mediator.Send(new GetAgreementRequest(), cancellationToken);
                _cacheService.Set(cacheKey, response, GlobalConstants.ENDPOINT_MASTER_STATIC_MINUTE);
            }

            return Ok(response);


            //var response = await _mediator.Send(new GetAgreementRequest(), cancellationToken);
            //return Ok(response);

        }



        [HttpPost("create")]
        public async Task<ActionResult> Create(CreateAgreementRequest request, CancellationToken cancellationToken)
        {

            var response = await _mediator.Send(request, cancellationToken);
            var cacheKey = $"GetAgreement";
            _cacheService.Remove(cacheKey);
            return Ok(response);

        }



        [HttpPost("agreementcheck")]
        public async Task<ActionResult> AgreementCheck(AgreementCheckRequest request, CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(request, cancellationToken);
            return Ok(response);

        }



    }
}
