using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using tas.Application.Features.CostCodeFeature.BulkDownloadCostCode;
using tas.Application.Features.CostCodeFeature.BulkDownloadCostCodeEmployees;
using tas.Application.Features.CostCodeFeature.BulkUploadCostCode;
using tas.Application.Features.CostCodeFeature.BulkUploadCostCodeEmployees;
using tas.Application.Features.CostCodeFeature.BulkUploadPreviewCostCode;
using tas.Application.Features.CostCodeFeature.BulkUploadPreviewCostCodeEmployees;
using tas.Application.Features.CostCodeFeature.CreateCostCode;
using tas.Application.Features.CostCodeFeature.DeleteCostCode;
using tas.Application.Features.CostCodeFeature.GetAllCostCode;
using tas.Application.Features.CostCodeFeature.UpdateCostCode;
using tas.Application.Features.DepartmentFeature.GetAllDepartment;
using tas.Application.Features.RoomFeature.BulkDownloadRoom;
using tas.Application.Features.RoomFeature.BulkUploadRoom;
using tas.Application.Features.UserFeatures.GetAllUser;
using tas.Application.Repositories;
using tas.Application.Service;
using tas.Domain.Common;
using tas.Domain.Entities;

namespace tas.WebAPI.Controllers.Tas
{
    [Route("api/tas/[controller]")]
    [ApiController]
    [RoleAuthorize]
    public class CostCodeController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<CostCodeController> _logger;
        private readonly IMemoryCache _memoryCache;
        private readonly HTTPUserRepository _hTTPUserRepository;

        public CostCodeController(IMediator mediator, ILogger<CostCodeController> logger, IMemoryCache memoryCache, HTTPUserRepository hTTPUserRepository)
        {
            _mediator = mediator;
            _logger = logger;
            _memoryCache = memoryCache;
            _hTTPUserRepository = hTTPUserRepository;   
        }

        [HttpGet]
        public async Task<ActionResult<List<GetAllCostCodeResponse>>> GetAll(int? active, CancellationToken cancellationToken)
        {
            var outData = new List<GetAllCostCodeResponse>();
            string cacheEntityName = typeof(CostCode).Name;
            if (active == null)
            {
                if (_memoryCache.TryGetValue($"API::{cacheEntityName}", out outData))
                {
                    return Ok(outData);
                }
                else
                {

                    var response = await _mediator.Send(new GetAllCostCodeRequest(null), cancellationToken);
                    _memoryCache.Set($"API::{cacheEntityName}", response, TimeSpan.FromMinutes(GlobalConstants.ENDPOINT_MASTER_CACHE_MINUTE));
                    return Ok(response);

                }
            }
            else
            {
                if (_memoryCache.TryGetValue($"API::{cacheEntityName}_{active}", out outData))
                {
                    return Ok(outData);
                }
                else
                {
                    var response = await _mediator.Send(new GetAllCostCodeRequest(active), cancellationToken);
                    _memoryCache.Set($"API::{cacheEntityName}_{active}", response, TimeSpan.FromMinutes(GlobalConstants.ENDPOINT_MASTER_CACHE_MINUTE));
                    return Ok(response);

                }
            }


        }

        [HttpPost]
        public async Task<ActionResult> Create(CreateCostCodeRequest request,
            CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(request, cancellationToken);
            _hTTPUserRepository.ClearAllMasterCache<CostCode>();
            //_memoryCache.Remove($"API::{typeof(CostCode).Name}_1");
            //_memoryCache.Remove($"API::{typeof(CostCode).Name}_0");
            //_memoryCache.Remove($"API::{typeof(CostCode).Name}");
            return Ok(response);
        }


        [HttpPut]
        public async Task<ActionResult>Update(UpdateCostCodeRequest request,
            CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(request, cancellationToken);
            _hTTPUserRepository.ClearAllMasterCache<CostCode>();
            return Ok(response);
        }

        [HttpDelete]
        public async Task<ActionResult> Delete(DeleteCostCodeRequest request,
            CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(request, cancellationToken);
            _hTTPUserRepository.ClearAllMasterCache<CostCode>();
            return Ok(response);
        }

        [HttpPost("bulkrequest")]
        public async Task<ActionResult> BulkRequest(BulkDownloadCostCodeRequest request,
CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(request, cancellationToken);
            return File(response.ExcelFile, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"TAS_CostCode_Bulk_Download_{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}.xlsx"); ;
        }



        [HttpPost("bulkrequestemployees")]
        public async Task<ActionResult> BulkEmployeesRequest(BulkDownloadCostCodeEmployeesRequest request,
CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(request, cancellationToken);
            return File(response.ExcelFile, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"TAS_CostCode_Employee_Bulk_Download_{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}.xlsx"); ;
        }




        [HttpPost("bulkuploademployees")]
        public async Task<ActionResult> BulkUpload([FromForm] BulkUploadCostCodeEmployeesRequest request,
CancellationToken cancellationToken)
        {
            await _mediator.Send(request, cancellationToken);
            _hTTPUserRepository.ClearAllEmployeeCache();
            _hTTPUserRepository.ClearAllMasterCache<CostCode>();
            return Ok();
        }


        [HttpPost("bulkuploademployeespreview")]
        public async Task<ActionResult> BulkUploadEmployeePreview([FromForm] BulkUploadPreviewCostCodeEmployeesRequest request,
CancellationToken cancellationToken)
        {
           var returndata =  await _mediator.Send(request, cancellationToken);
            return Ok(returndata);
        }


        [HttpPost("bulkupload")]
        public async Task<ActionResult> BulkUpload([FromForm] BulkUploadCostCodeRequest request,
CancellationToken cancellationToken)
        {
             await _mediator.Send(request, cancellationToken);
            _hTTPUserRepository.ClearAllMasterCache<CostCode>();
            return Ok();
        }


        [HttpPost("bulkuploadpreview")]
        public async Task<ActionResult> BulkUploadPreview([FromForm] BulkUploadPreviewCostCodeRequest request,
CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(request, cancellationToken);
            return Ok(response);
        }



    }
}
