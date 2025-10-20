using DocumentFormat.OpenXml.Wordprocessing;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using tas.Application.Features.DepartmenteFeature.BulkUploadPreviewDepartmentEmployees;
using tas.Application.Features.DepartmentFeature.AddDepartmentAdmin;
using tas.Application.Features.DepartmentFeature.AddDepartmentManager;
using tas.Application.Features.DepartmentFeature.AddDepartmentSupervisor;
using tas.Application.Features.DepartmentFeature.BulkDownloadDepartment;
using tas.Application.Features.DepartmentFeature.BulkDownloadDepartmentEmployees;
using tas.Application.Features.DepartmentFeature.BulkUploadDepartment;
using tas.Application.Features.DepartmentFeature.BulkUploadDepartmentEmployees;
using tas.Application.Features.DepartmentFeature.CreateDepartment;
using tas.Application.Features.DepartmentFeature.CustomListDepartmment;
using tas.Application.Features.DepartmentFeature.DeleteDepartment;
using tas.Application.Features.DepartmentFeature.DeleteDepartmentAdmin;
using tas.Application.Features.DepartmentFeature.DeleteDepartmentManager;
using tas.Application.Features.DepartmentFeature.DeleteDepartmentSupervisor;
using tas.Application.Features.DepartmentFeature.GetAdminsDepartment;
using tas.Application.Features.DepartmentFeature.GetAllDepartment;
using tas.Application.Features.DepartmentFeature.GetAllDepartmentAdmins;
using tas.Application.Features.DepartmentFeature.GetAllDepartmentManagers;
using tas.Application.Features.DepartmentFeature.GetAllReportDepartment;
using tas.Application.Features.DepartmentFeature.GetDepartment;
using tas.Application.Features.DepartmentFeature.GetManagersDepartment;
using tas.Application.Features.DepartmentFeature.GetParentDepartments;
using tas.Application.Features.DepartmentFeature.SetMainDepartmentAdmin;
using tas.Application.Features.DepartmentFeature.SetMainDepartmentManager;
using tas.Application.Features.DepartmentFeature.SetMainDepartmentSupervisor;
using tas.Application.Features.DepartmentFeature.UpdateDepartment;
using tas.Application.Features.PeopleTypeFeature.GetAllPeopleType;
using tas.Application.Features.RoomFeature.BulkDownloadRoom;
using tas.Application.Repositories;
using tas.Application.Service;
using tas.Domain.Common;
using tas.Domain.Entities;

namespace tas.WebAPI.Controllers.Tas
{

    [Route("api/tas/[controller]")]
    [ApiController]
    [RoleAuthorize]
    public class DepartmentController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<DepartmentController> _logger;
        private readonly HTTPUserRepository _HTTPUserRepository;

        private readonly CacheService _cacheService;

        //    private readonly IMemoryCache _memoryCache;

        public DepartmentController(IMediator mediator, ILogger<DepartmentController> logger, IMemoryCache memoryCache, HTTPUserRepository hTTPUserRepository, CacheService cacheService)
        {
            _mediator = mediator;
            _logger = logger;
          //  _memoryCache = memoryCache;
            _HTTPUserRepository = hTTPUserRepository;
            _cacheService = cacheService;
        }

        [HttpGet]
        public async Task<ActionResult<List<GetAllDepartmentResponse>>> GetAll(string? DepartmentName, string? keyword,  CancellationToken cancellationToken)
        {

            //   var response = await _mediator.Send(new GetAllDepartmentRequest(DepartmentName, keyword), cancellationToken);
            // _memoryCache.Set($"API::{cacheEntityName}_{active}", response, TimeSpan.FromMinutes(GlobalConstants.ENDPOINT_MASTER_CACHE_MINUTE));
            var outData = new List<GetAllDepartmentResponse>();
            string cacheEntityName = typeof(Department).Name;

            //string cacheEntityName = typeof(Department).Name;
            //if (_memoryCache.TryGetValue($"API::{cacheEntityName}_{DepartmentName}", out outData))
            //{
            //    return Ok(outData);
            //}
            //else
            //{
            //    var response = await _mediator.Send(new GetAllDepartmentRequest(DepartmentName, keyword), cancellationToken);
            //    _memoryCache.Set($"API::{cacheEntityName}_{DepartmentName}", response, TimeSpan.FromMinutes(GlobalConstants.ENDPOINT_MASTER_CACHE_MINUTE));
            //    return Ok(response);

            //}

            var cacheKey = $"API::{cacheEntityName}_{DepartmentName}_{keyword}";

            if (_cacheService.TryGetValue(cacheKey, out outData))
            {
                Console.WriteLine("FROM CACHE");
                return Ok(outData);
            }
            else
            {
                var response = await _mediator.Send(new GetAllDepartmentRequest(DepartmentName, keyword), cancellationToken);
                _cacheService.Set(cacheKey, response,GlobalConstants.ENDPOINT_MASTER_CACHE_MINUTE);
                Console.WriteLine("FROM DB");
                return Ok(response);
            }

        }


        [HttpGet("report")]
        public async Task<ActionResult<List<GetAllDepartmentResponse>>> GetReportAll(CancellationToken cancellationToken)
        {

            var outData = new List<GetAllDepartmentResponse>();
            string cacheEntityName = typeof(Department).Name;
            var cacheKey = $"API::_REPORT_{cacheEntityName}";
            if (_cacheService.TryGetValue(cacheKey, out outData))
            {
                Console.WriteLine("FROM CACHE");
                return Ok(outData);
            }
            else
            {
                var response = await _mediator.Send(new GetAllReportDepartmentRequest(), cancellationToken);
                _cacheService.Set(cacheKey, response, GlobalConstants.ENDPOINT_MASTER_CACHE_MINUTE);
                Console.WriteLine("FROM DB");
                return Ok(response);
            }

        }


        [HttpGet("parent/{Id}")]
        public async Task<ActionResult<List<GetParentDepartmentsResponse>>> GetGetParentDepartments(int Id, CancellationToken cancellationToken)
        {
            var outData = new List<GetParentDepartmentsResponse>();
            string cacheEntityName = typeof(Department).Name;


            var cacheKey = $"API::{cacheEntityName}_parent_{Id}";

            if (_cacheService.TryGetValue(cacheKey, out outData))
            {
                Console.WriteLine("FROM CACHE");
                return Ok(outData);
            }
            else
            {
                var response = await _mediator.Send(new GetParentDepartmentsRequest(Id), cancellationToken);
                _cacheService.Set(cacheKey, response, TimeSpan.FromMinutes(GlobalConstants.ENDPOINT_MASTER_CACHE_MINUTE));
                return Ok(response);
            }

        }

        [HttpGet("{Id}")]
        public async Task<ActionResult<GetDepartmentResponse>> Get(int Id, CancellationToken cancellationToken)
        { 
            var response = await _mediator.Send(new GetDepartmentRequest(Id), cancellationToken);
            return Ok(response);

        }

        [HttpPost("manager")]
        public async Task<ActionResult> AddDepartmentManager(AddDepartmentManagerRequest request, CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(request, cancellationToken);
            return Ok(response);

        }

        [HttpPost("supervisor")]
        public async Task<ActionResult> AddDepartmentSupervisor(AddDepartmentSupervisorRequest request, CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(request, cancellationToken);
            return Ok(response);

        }

        [HttpPost("admin")]
        public async Task<ActionResult> AddDepartmentAdmin(AddDepartmentAdminRequest request, CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(request, cancellationToken);
            return Ok(response);

        }


        [HttpDelete("manager/{Id}")]
        public async Task<ActionResult> DeleteDepartmentManager(int Id, CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(new DeleteDepartmentManagerRequest(Id) , cancellationToken);
            return Ok(response);

        }

        [HttpDelete("admin/{Id}")]
        public async Task<ActionResult> DeleteDepartmentAdmin(int Id, CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(new DeleteDepartmentAdminRequest(Id), cancellationToken);
            return Ok(response);

        }

        [HttpDelete("supervisor/{Id}")]
        public async Task<ActionResult> DeleteDepartmentSupervisor(int Id, CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(new DeleteDepartmentSupervisorRequest(Id), cancellationToken);
            return Ok(response);

        }


        [HttpGet("minimum")]
        public async Task<ActionResult<List<CustomListDepartmentResponse>>> GetMinimlist (CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(new CustomListDepartmentRequest(), cancellationToken);
            return Ok(response);
        }

        
        [HttpGet("admins")]
        public async Task<ActionResult<List<GetAllDepartmentAdminsResponse>>> GetAdmins(CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(new GetAllDepartmentAdminsRequest(), cancellationToken);
            return Ok(response);
        }
         
        [HttpGet("managers")]
        public async Task<ActionResult<List<GetAllDepartmentManagersResponse>>> GetManagers(CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(new GetAllDepartmentManagersRequest(), cancellationToken);
            return Ok(response);
        }


        [HttpPut("setmainmanager")]
        public async Task<ActionResult> SetMainDepartmentManager(int Id, CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(new SetMainDepartmentManagerRequest(Id), cancellationToken);
            return Ok(response);
        }


        [HttpPut("setmainadmin")]
        public async Task<ActionResult> SetMainDepartmentAdmin(int Id, CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(new SetMainDepartmentAdminRequest(Id), cancellationToken);
            return Ok(response);
        }

        [HttpPut("setmainsupervisor")]
        public async Task<ActionResult> SetMainDepartmentSupervisor(int Id, CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(new SetMainDepartmentSupervisorRequest(Id), cancellationToken);
            return Ok(response);
        }


        [HttpGet("managerdepartments/{EmployeeId}")]
        public async Task<ActionResult<List<GetManagersDepartmentResponse>>> GetManagersDepartment(int EmployeeId, CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(new GetManagersDepartmentRequest(EmployeeId), cancellationToken);
            return Ok(response);
        }


        [HttpGet("admindepartments/{EmployeeId}")]
        public async Task<ActionResult<List<GetAdminsDepartmentResponse>>> GetAdminsDepartment(int EmployeeId, CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(new GetAdminsDepartmentRequest(EmployeeId), cancellationToken);
            return Ok(response);
        }

        [HttpPost]
        public async Task<ActionResult> Create(CreateDepartmentRequest request,
            CancellationToken cancellationToken)
        {

            var response = await _mediator.Send(request, cancellationToken);
            _HTTPUserRepository.ClearAllMasterCache<Department>();
            return Ok(response);
        }


        [HttpPut]
        public async Task<ActionResult> Update(UpdateDepartmentRequest request,
            CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(request, cancellationToken);
            _HTTPUserRepository.ClearAllMasterCache<Department>();
            return Ok(response);
        }

        [HttpDelete]
        public async Task<ActionResult> Delete(DeleteDepartmentRequest request,
            CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(request, cancellationToken);
            _HTTPUserRepository.ClearAllMasterCache<Department>();
            return Ok(response);
        }


        [HttpPost("bulkrequest")]
        public async Task<ActionResult> BulkRequest(BulkDownloadDepartmentRequest request,
CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(request, cancellationToken);
            string fileName = $"TAS_Department_Bulk_Download_{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}.xlsx";
            HttpContext.Response.Headers.Add("FileName", fileName);
            return File(response.ExcelFile, "application/force-download", fileName);
        }

        [HttpPost("bulkrequestemployees")]
        public async Task<ActionResult> BulkRequestEmployees(BulkDownloadDepartmentEmployeesRequest request,
CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(request, cancellationToken);
            string fileName = $"TAS_Department_Employees_Bulk_Download_{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}.xlsx";
            HttpContext.Response.Headers.Add("FileName", fileName);
            return File(response.ExcelFile, "application/force-download", fileName);
        }


        [HttpPost("bulkuploademployees")]
        public async Task<ActionResult> BulkUpload([FromForm] BulkUploadDepartmentEmployeesRequest request,
CancellationToken cancellationToken)
        {
            await _mediator.Send(request, cancellationToken);
            _HTTPUserRepository.ClearAllEmployeeCache();
            _HTTPUserRepository.ClearAllMasterCache<Department>();
            return Ok();
        }

        [HttpPost("bulkuploademployeespreview")]
        public async Task<ActionResult> BulkUploadEmployeePreview([FromForm] BulkUploadPreviewDepartmentEmployeesRequest request,
CancellationToken cancellationToken)
        {
            var returndata = await _mediator.Send(request, cancellationToken);
            return Ok(returndata);
        }


        [HttpPost("bulkupload")]
        public async Task<ActionResult> BulkUpload([FromForm] BulkUploadDepartmentRequest request,
CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(request, cancellationToken);
            _HTTPUserRepository.ClearAllMasterCache<Department>();
            return Ok(response);
        }




    }
}
