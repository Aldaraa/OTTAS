using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using tas.Application.Features.CostCodeFeature.BulkDownloadCostCode;
using tas.Application.Features.CostCodeFeature.BulkDownloadCostCodeEmployees;
using tas.Application.Features.CostCodeFeature.BulkUploadCostCode;
using tas.Application.Features.CostCodeFeature.BulkUploadCostCodeEmployees;
using tas.Application.Features.CostCodeFeature.BulkUploadPreviewCostCode;
using tas.Application.Features.CostCodeFeature.BulkUploadPreviewCostCodeEmployees;
using tas.Application.Features.CostCodeFeature.GetAllCostCode;
using tas.Application.Features.EmployeeFeature.ChangeEmployeeData;
using tas.Application.Features.EmployerFeature.BulkDownloadEmployer;
using tas.Application.Features.EmployerFeature.BulkDownloadEmployerEmployees;
using tas.Application.Features.EmployerFeature.BulkUploadEmployer;
using tas.Application.Features.EmployerFeature.BulkUploadEmployerEmployees;
using tas.Application.Features.EmployerFeature.BulkUploadPreviewEmployer;
using tas.Application.Features.EmployerFeature.BulkUploadPreviewEmployerEmployees;
using tas.Application.Features.EmployerFeature.CreateEmployer;
using tas.Application.Features.EmployerFeature.DeleteEmployer;
using tas.Application.Features.EmployerFeature.GetAllEmployer;
using tas.Application.Features.EmployerFeature.GetAllReportEmployer;
using tas.Application.Features.EmployerFeature.UpdateEmployer;
using tas.Application.Repositories;
using tas.Application.Service;
using tas.Domain.Common;
using tas.Domain.Entities;

namespace tas.WebAPI.Controllers.Tas
{
    [Route("api/tas/[controller]")]
    [ApiController]
    [RoleAuthorize]
    public class EmployerController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<EmployerController> _logger;
        private readonly IMemoryCache _memoryCache;
        private readonly CacheService _cacheService;
        private readonly HTTPUserRepository _hTTPUserRepository;

        public EmployerController(IMediator mediator, ILogger<EmployerController> logger, IMemoryCache memoryCache, HTTPUserRepository hTTPUserRepository, CacheService cacheService)
        {
            _mediator = mediator;
            _logger = logger;
            _hTTPUserRepository = hTTPUserRepository;
            _memoryCache = memoryCache;
            _cacheService = cacheService;
        }

        [HttpGet]
        public async Task<ActionResult<List<GetAllEmployerResponse>>> GetAll(int? active, CancellationToken cancellationToken)
        {

            var outData = new List<GetAllEmployerResponse>();
            string cacheEntityName = typeof(Employer).Name;
            if (active == null)
            {
                if (_cacheService.TryGetValue($"API::{cacheEntityName}", out outData))
                {
                    return Ok(outData);
                }
                else
                {

                    var response = await _mediator.Send(new GetAllEmployerRequest(null), cancellationToken);
                    _cacheService.Set($"API::{cacheEntityName}", response, TimeSpan.FromMinutes(GlobalConstants.ENDPOINT_MASTER_CACHE_MINUTE));
                    return Ok(response);

                }
            }
            else
            {
                if (_cacheService.TryGetValue($"API::{cacheEntityName}_{active}", out outData))
                {
                    return Ok(outData);
                }
                else
                {
                    var response = await _mediator.Send(new GetAllEmployerRequest(active), cancellationToken);
                    _cacheService.Set($"API::{cacheEntityName}_{active}", response, TimeSpan.FromMinutes(GlobalConstants.ENDPOINT_MASTER_CACHE_MINUTE));
                    return Ok(response);

                }
            }

        }


        [HttpGet("report")]
        public async Task<ActionResult<List<GetAllEmployerResponse>>> GetReportAll(CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(new GetAllReportEmployerRequest(), cancellationToken);
           // _cacheService.Set($"API::{cacheEntityName}", response, TimeSpan.FromMinutes(GlobalConstants.ENDPOINT_MASTER_CACHE_MINUTE));
            return Ok(response);

        }

        [HttpPost]
        public async Task<ActionResult> Create(CreateEmployerRequest request,
            CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(request, cancellationToken);
            _hTTPUserRepository.ClearAllMasterCache<Employer>();
            return Ok(response);
        }


        [HttpPut]
        public async Task<ActionResult> Update(UpdateEmployerRequest request,
            CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(request, cancellationToken);
            _hTTPUserRepository.ClearAllMasterCache<Employer>();
            return Ok(response);
        }

        [HttpDelete]
        public async Task<ActionResult> Delete(DeleteEmployerRequest request,
            CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(request, cancellationToken);
            _hTTPUserRepository.ClearAllMasterCache<Employer>();
            return Ok(response);
        }


        [HttpPost("bulkrequest")]
        public async Task<ActionResult> BulkRequest(BulkDownloadEmployerRequest request,
CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(request, cancellationToken);
            return File(response.ExcelFile, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"TAS_CostCode_Bulk_Download_{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}.xlsx"); ;
        }


        [HttpPost("bulkupload")]
        public async Task<ActionResult> BulkUpload([FromForm] BulkUploadEmployerRequest request,
CancellationToken cancellationToken)
        {
            await _mediator.Send(request, cancellationToken);
            _hTTPUserRepository.ClearAllMasterCache<Employer>();
            return Ok();
        }


        [HttpPost("bulkuploadpreview")]
        public async Task<ActionResult> BulkUploadPreview([FromForm] BulkEmployerUploadPreviewRequest request,
CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(request, cancellationToken);
            return Ok(response);
        }


        [HttpPost("bulkrequestemployees")]
        public async Task<ActionResult> BulkEmployeesRequest(BulkDownloadEmployerEmployeesRequest request,
CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(request, cancellationToken);
            return File(response.ExcelFile, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"TAS_Employer_Employee_Bulk_Download_{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}.xlsx"); ;
        }




        [HttpPost("bulkuploademployees")]
        public async Task<ActionResult> BulkUpload([FromForm] BulkUploadEmployerEmployeesRequest request,
CancellationToken cancellationToken)
        {
            await _mediator.Send(request, cancellationToken);
            _hTTPUserRepository.ClearAllEmployeeCache();
            _hTTPUserRepository.ClearAllMasterCache<Employer>();
            return Ok();
        }


        [HttpPost("bulkuploademployeespreview")]
        public async Task<ActionResult> BulkUploadEmployeePreview([FromForm] BulkEmployerUploadPreviewEmployeesRequest request,
CancellationToken cancellationToken)
        {
            var returndata = await _mediator.Send(request, cancellationToken);
            return Ok(returndata);
        }



    }
}
