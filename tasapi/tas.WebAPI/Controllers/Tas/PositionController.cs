using DocumentFormat.OpenXml.Wordprocessing;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using Microsoft.Extensions.Caching.Memory;
using tas.Application.Features.PositioneFeature.BulkUploadPosition;
using tas.Application.Features.PositioneFeature.BulkUploadPreviewPosition;
using tas.Application.Features.PositionFeature.AllPosition;
using tas.Application.Features.PositionFeature.BulkDownloadPosition;
using tas.Application.Features.PositionFeature.BulkDownloadPositionEmployees;
using tas.Application.Features.PositionFeature.BulkUploadPositionEmployees;
using tas.Application.Features.PositionFeature.BulkUploadPreviewPositionEmployees;
using tas.Application.Features.PositionFeature.CreatePosition;
using tas.Application.Features.PositionFeature.DeletePosition;
using tas.Application.Features.PositionFeature.GetAllPosition;
using tas.Application.Features.PositionFeature.UpdatePosition;
using tas.Application.Features.StateFeature.GetAllState;
using tas.Application.Repositories;
using tas.Application.Service;
using tas.Domain.Common;
using tas.Domain.Entities;
using Position = tas.Domain.Entities.Position;

namespace tas.WebAPI.Controllers.Tas
{

    [Route("api/tas/[controller]")]
    [ApiController]
    [RoleAuthorize]
    public class PositionController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<PositionController> _logger;
        private readonly CacheService _memoryCache;
        private readonly HTTPUserRepository _hTTPUserRepository;

        public PositionController(IMediator mediator, ILogger<PositionController> logger, CacheService memoryCache, HTTPUserRepository hTTPUserRepository)
        {
            _mediator = mediator;
            _logger = logger;
            _memoryCache = memoryCache;
            _hTTPUserRepository = hTTPUserRepository;
        }

        [HttpPost("getall")]
        public async Task<ActionResult<List<GetAllPositionResponse>>> GetAll(GetAllPositionRequest request, string? sortBy,
        string? sortDirection, CancellationToken cancellationToken)
        {
            if (!string.IsNullOrEmpty(sortBy))
            {
                var response = await _mediator.Send(request, cancellationToken);
                var property = typeof(GetAllPositionResult).GetProperty(sortBy);
                    if (property != null)
                    {
                        if (sortDirection == "asc")
                        {

                            response.data = response.data.OrderBy(item => property.GetValue(item)?.ToString()).ToList();
                        }
                        else
                        {
                        response.data = response.data.OrderByDescending(item => property.GetValue(item)?.ToString()).ToList();
                        }
                    }

                    return Ok(response);
                



            }
            else {
                    var response = await _mediator.Send(request, cancellationToken);
                    return Ok(response);
            }
            //return Ok(response);


            //var outData = new List<GetAllPositionResponse>();
            //string cacheEntityName = typeof(Position).Name;
            //if (string.IsNullOrEmpty(sortBy))
            //{
            //    if (_memoryCache.TryGetValue($"API::{cacheEntityName}", out outData))
            //    {
            //        return Ok(outData);
            //    }
            //    else
            //    {

            //        var response = await _mediator.Send(new GetAllStateRequest(null), cancellationToken);
            //        _memoryCache.Set($"API::{cacheEntityName}", response, TimeSpan.FromMinutes(GlobalConstants.ENDPOINT_MASTER_CACHE_MINUTE));
            //        return Ok(response);

            //    }
            //}
            //else
            //{
            //    if (_memoryCache.TryGetValue($"API::{cacheEntityName}_{sortBy}", out outData))
            //    {
            //        return Ok(outData);
            //    }
            //    else
            //    {
            //        var response = await _mediator.Send(new GetAllStateRequest(active), cancellationToken);
            //        _memoryCache.Set($"API::{cacheEntityName}_{active}", response, TimeSpan.FromMinutes(GlobalConstants.ENDPOINT_MASTER_CACHE_MINUTE));
            //        return Ok(response);

            //    }
            //}







        }


        [HttpGet]
        [OutputCache(Duration = 300, VaryByQueryKeys = new string[] { "active" })]
        public async Task<ActionResult<List<AllPositionResponse>>> GetAllData(int? active, CancellationToken cancellationToken)
        {

            if (active == null)
            {
                var response = await _mediator.Send(new AllPositionRequest(null), cancellationToken);
                return Ok(response);
            }
            else
            {
                var response = await _mediator.Send(new AllPositionRequest(active), cancellationToken);
                return Ok(response);

            }
        }

        [HttpPost]
        public async Task<ActionResult> Create(CreatePositionRequest request,
            CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(request, cancellationToken);
            return Ok(response);
        }


        [HttpPut]
        public async Task<ActionResult> Update(UpdatePositionRequest request,
            CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(request, cancellationToken);
            return Ok(response);
        }

        [HttpDelete]
        public async Task<ActionResult> Delete(DeletePositionRequest request,
            CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(request, cancellationToken);
            return Ok(response);
        }



        [HttpPost("bulkrequest")]
        public async Task<ActionResult> BulkRequest(BulkDownloadPositionRequest request,
CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(request, cancellationToken);
            return File(response.ExcelFile, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"TAS_Position_Bulk_Download_{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}.xlsx"); ;
        }


        [HttpPost("bulkupload")]
        public async Task<ActionResult> BulkUpload([FromForm] BulkUploadPositionRequest request,
CancellationToken cancellationToken)
        {
             await _mediator.Send(request, cancellationToken);
            return Ok();
        }

        [HttpPost("bulkuploadpreview")]
        public async Task<ActionResult> BulkUploadPreview([FromForm] BulkRequestUploadPreviewPositionRequest request,
CancellationToken cancellationToken)
        {
           var data =  await _mediator.Send(request, cancellationToken);
            return Ok(data);
        }


        [HttpPost("bulkrequestemployees")]
        public async Task<ActionResult> BulkEmployeesRequest(BulkDownloadPositionEmployeesRequest request,
CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(request, cancellationToken);
            return File(response.ExcelFile, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"TAS_Position_Employee_Bulk_Download_{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}.xlsx"); ;
        }




        [HttpPost("bulkuploademployees")]
        public async Task<ActionResult> BulkUpload([FromForm] BulkUploadPositionEmployeesRequest request,
CancellationToken cancellationToken)
        {
            await _mediator.Send(request, cancellationToken);
            _hTTPUserRepository.ClearAllEmployeeCache();
            _hTTPUserRepository.ClearAllMasterCache<Position>();
            return Ok();
        }


        [HttpPost("bulkuploademployeespreview")]
        public async Task<ActionResult> BulkUploadEmployeePreview([FromForm] BulkUploadPreviewPositionEmployeesRequest request,
CancellationToken cancellationToken)
        {
            var returndata = await _mediator.Send(request, cancellationToken);
            return Ok(returndata);
        }


    }
}
