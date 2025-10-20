using Application.Common.Utils;
using Application.Features.ReportTemplateFeature.DateSimulation;
using Application.Features.ReportTemplateFeature.GetAllReportTemplate;
using Application.Features.ReportTemplateFeature.GetDashboard;
using Application.Features.ReportTemplateFeature.GetReportDateVariables;
using Application.Features.ReportTemplateFeature.GetReportTemplateData;
using Application.Features.ReportTemplateFeature.GetReportTemplateMaster;
using Application.Features.ReportTemplateFeature.UpdateTemplateParameter;
using Application.Service;
using Domain.CustomModel;
using MediatR;
using Microsoft.AspNetCore.Authentication.Negotiate;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace tas.ReportAPI.Controllers
{
    [Route("rapi/tasreport/[controller]")]
    [ApiController]

    [Authorize(AuthenticationSchemes = NegotiateDefaults.AuthenticationScheme)]
    public class ReportTemplateController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IConfiguration _configuration;
        private readonly IMemoryCache _memoryCache;
        public ReportTemplateController(IMediator mediator, IConfiguration configuration, IMemoryCache memoryCache)
        {
            _mediator = mediator;
            _memoryCache = memoryCache;
        }

        [HttpGet]
        public async Task<ActionResult<List<GetAllReportTemplateResponse>>> GetAll(CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(new GetAllReportTemplateRequest(1), cancellationToken);
            return Ok(response);
        }

        [HttpGet("{templateId}")]
        public async Task<ActionResult<GetReportTemplateDataResponse>> Get(int templateId, CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(new GetReportTemplateDataRequest(templateId), cancellationToken);
            return Ok(response);
        }


        [HttpPut("parameter")]
        public async Task<ActionResult> UpdateParameter(UpdateTemplateParameterRequest request, CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(request, cancellationToken);
            return Ok(response);
        }


        [HttpGet("masters")]
        public async Task<ActionResult<GetReportTemplateDataResponse>> GetMaster(CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(new GetReportTemplateMasterRequest(), cancellationToken);
            return Ok(response);
        }


        [HttpGet("datetypes")]
        public async Task<ActionResult<GetReportDateVariablesResponse>> GetDateTypes(CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(new GetReportDateVariablesRequest(), cancellationToken);
            return Ok(response);
        }
        
        [HttpGet("datesimulate/{datetype}/{day}")]
        public async Task<ActionResult> GetDateSimulate(string datetype, int day, CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(new DateSimulationRequest(datetype, day), cancellationToken);
            return Ok(response);
        }

        [HttpGet("dashboard")]
        public async Task<ActionResult<GetReportTemplateDataResponse>> GetDashboard(CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(new GetDashboardRequest(), cancellationToken);
            return Ok(response);
        }



        [HttpGet("test")]

        [Authorize(AuthenticationSchemes = NegotiateDefaults.AuthenticationScheme)]
        public async Task<ActionResult> login()
        {
            string? username = HttpContext.User.Identity.Name;
            var outData = new AuthUser();


            if (!_memoryCache.TryGetValue($"REPORT_AUTH_{username}", out outData))
            {
                return Ok(outData);
            }
            else {
                return Ok(outData);
            }
        }
    }
}
