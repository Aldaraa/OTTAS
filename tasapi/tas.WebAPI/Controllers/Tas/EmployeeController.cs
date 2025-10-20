
using DocumentFormat.OpenXml.Office2010.Excel;
using MediatR;
using Microsoft.AspNetCore.Authentication.Negotiate;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using System.Collections;
using System.Reflection;
using tas.Application.Features.AuthenticationFeature.LoginUser;
using tas.Application.Features.BedFeature.GetBed;
using tas.Application.Features.DashboardFeature.EmployeeDashboard;
using tas.Application.Features.DepartmentFeature.GetAllDepartment;
using tas.Application.Features.EmployeeFeature.ActiveEmployeeDirectRequest;
using tas.Application.Features.EmployeeFeature.BulkDownloadEmployee;
using tas.Application.Features.EmployeeFeature.BulkDownloadGroupEmployee;
using tas.Application.Features.EmployeeFeature.BulkUploadEmployee;
using tas.Application.Features.EmployeeFeature.BulkUploadEmployeeGroup;
using tas.Application.Features.EmployeeFeature.BulkUploadPreviewEmployee;
using tas.Application.Features.EmployeeFeature.BulkUploadPreviewEmployeeGroup;
using tas.Application.Features.EmployeeFeature.ChangeEmployeeData;
using tas.Application.Features.EmployeeFeature.ChangeEmployeeDataGroup;
using tas.Application.Features.EmployeeFeature.ChangeEmployeeLocation;
using tas.Application.Features.EmployeeFeature.CheckADAccountEmployee;
using tas.Application.Features.EmployeeFeature.CreateEmployee;
using tas.Application.Features.EmployeeFeature.CreateEmployeeRequest;
using tas.Application.Features.EmployeeFeature.DeActiveEmployee;
using tas.Application.Features.EmployeeFeature.DeleteEmployee;
using tas.Application.Features.EmployeeFeature.DeleteEmployeeTransport;
using tas.Application.Features.EmployeeFeature.DeleteEmployeeTransportBulk;
using tas.Application.Features.EmployeeFeature.EmployeeDeActiveDateCheck;
using tas.Application.Features.EmployeeFeature.EmployeeDeActiveDateCheckMultiple;
using tas.Application.Features.EmployeeFeature.GetAllEmployee;
using tas.Application.Features.EmployeeFeature.GetEmployee;
using tas.Application.Features.EmployeeFeature.GetEmployeeAccountHistory;
using tas.Application.Features.EmployeeFeature.GetProfileTransport;
using tas.Application.Features.EmployeeFeature.ReActiveEmployee;
using tas.Application.Features.EmployeeFeature.RemovePassportImageEmployee;
using tas.Application.Features.EmployeeFeature.RosterExecuteEmployee;
using tas.Application.Features.EmployeeFeature.RosterExecutePreviewEmployee;
using tas.Application.Features.EmployeeFeature.SearchEmployee;
using tas.Application.Features.EmployeeFeature.SearchEmployeeAccommodation;
using tas.Application.Features.EmployeeFeature.SearchShortEmployee;
using tas.Application.Features.EmployeeFeature.StatusEmployee;
using tas.Application.Features.EmployeeFeature.UpdateEmployee;
using tas.Application.Features.RoomFeature.BulkDownloadRoom;
using tas.Application.Features.RoomFeature.BulkUploadRoom;
using tas.Application.Repositories;
using tas.Application.Service;
using tas.Domain.Common;
using tas.Persistence.Repositories;

namespace tas.WebAPI.Controllers.Tas
{
    [Route("api/tas/[controller]")]
    [ApiController]
    [RoleAuthorize]
    public class EmployeeController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<EmployeeController> _logger;
        private readonly IConfiguration _configuration;
        private readonly NotificationHub _notificationHub;
        private readonly CacheService _memoryCache;
        private readonly HTTPUserRepository _hTTPUserRepository;

        public EmployeeController(IMediator mediator, ILogger<EmployeeController> logger, IConfiguration configuration, NotificationHub notificationHub, CacheService memoryCache, HTTPUserRepository hTTPUserRepository)
        {
            _mediator = mediator;
            _logger = logger;
            _configuration = configuration;
            _notificationHub = notificationHub;
            _memoryCache = memoryCache;
            _hTTPUserRepository = hTTPUserRepository;
        }



        [HttpGet("search")]
        [Authorize(AuthenticationSchemes = NegotiateDefaults.AuthenticationScheme)]
        public async Task<ActionResult<SearchEmployeeResponse>> ClearCache([FromQuery] bool clearCache = false)
        {
            if (clearCache)
            {
                string? username = HttpContext.User.Identity.Name;
                if (string.IsNullOrWhiteSpace(username))
                {
                    _hTTPUserRepository.ClearRoleCache(username);
                }
                    

                _hTTPUserRepository.ClearAllEmployeeCache();


            }

            return Ok();

        }



        [HttpPost("search")]
        public async Task<ActionResult<SearchEmployeeResponse>> Search(SearchEmployeeRequest request, string? sortBy,
        string? sortDirection, CancellationToken cancellationToken)
        {
            /*

            var roleName =  _hTTPUserRepository.LogCurrentUser()?.Role;
            if (roleName == "Supervisor" || roleName == "Guest")
            {
                var response = await _mediator.Send(request, cancellationToken);

                if (!string.IsNullOrEmpty(sortBy))
                {
                    var property = typeof(EmployeeSearchResult).GetProperty(sortBy);
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

                }

                Console.WriteLine("**************************from db***********************************");
                return Ok(response);


            }
            else {

                var Id = _hTTPUserRepository.LogCurrentUser()?.Id;

                string cacheEntityName = CacheKeyGenerator.GenerateCacheKeyEmployeeSearch(request, roleName, Id);

                var outData = new SearchEmployeeResponse();

                if (_memoryCache.TryGetValue(cacheEntityName, out outData))
                {
                    if (outData?.data.Count > 0)
                    {

                        if (!string.IsNullOrEmpty(sortBy))
                        {
                            var property = typeof(EmployeeSearchResult).GetProperty(sortBy);
                            if (property != null)
                            {
                                if (sortDirection == "asc")
                                {

                                    outData.data = outData.data.OrderBy(item => property.GetValue(item)?.ToString()).ToList();
                                }
                                else
                                {
                                    outData.data = outData.data.OrderByDescending(item => property.GetValue(item)?.ToString()).ToList();
                                }
                            }

                        }
                    }
                    else
                    {
                        var response = await _mediator.Send(request, cancellationToken);

                        _memoryCache.Set(cacheEntityName, response, TimeSpan.FromMinutes(GlobalConstants.ENDPOINT_TAS_DASHBOARD_CACHE_MINUTE));

                        if (!string.IsNullOrEmpty(sortBy))
                        {
                            var property = typeof(EmployeeSearchResult).GetProperty(sortBy);
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

                        }

                        Console.WriteLine("**************************from db***********************************");
                        return Ok(response);
                    }


                    Console.WriteLine("**************************from cached***********************************");
                    return Ok(outData);
                }
                else
                {
            */
                    
            var response = await _mediator.Send(request, cancellationToken);

//                    _memoryCache.Set(cacheEntityName, response, TimeSpan.FromMinutes(GlobalConstants.ENDPOINT_TAS_DASHBOARD_CACHE_MINUTE));

                    if (!string.IsNullOrEmpty(sortBy))
                    {
                        var property = typeof(EmployeeSearchResult).GetProperty(sortBy);
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

                    }

                    Console.WriteLine("**************************from db***********************************");
                    return Ok(response);

           //     }
       //     }


        }


        [HttpPost("searchaccommodation")]
        public async Task<ActionResult<SearchEmployeeAccommodationResponse>> SearchAccommodation(SearchEmployeeAccommodationRequest request, string? sortBy,
     string? sortDirection, CancellationToken cancellationToken)
        {
    
            var response = await _mediator.Send(request, cancellationToken);
            if (!string.IsNullOrEmpty(sortBy))
            {
                var property = typeof(SearchEmployeeAccommodationSearchResult).GetProperty(sortBy);
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

            }

            Console.WriteLine("**************************from db***********************************");
            return Ok(response);
        }

        [HttpPost("searchshort")]
        public async Task<ActionResult<SearchEmployeeResponse>> SearchShort(SearchShortEmployeeRequest request, CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(request, cancellationToken);
            return Ok(response);
        }


        [HttpPut("activedirectly/{employeeId}")]
        public async Task<ActionResult> DeleteEmployeeTransport(int employeeId, CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(new ActiveEmployeeDirectRequest(employeeId), cancellationToken);
            _hTTPUserRepository.ClearAllEmployeeCache();
            return Ok(response);
        }




        [HttpGet("profiletransport/{employeeId}")]
        public async Task<ActionResult> GetprofileTransport(int employeeId, CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(new GetProfileTransportRequest(employeeId), cancellationToken);
            return Ok(response);
        }




        [HttpDelete("deletetransport/{employeeId}/{onSiteDate}")]
        public async Task<ActionResult> DeleteEmployeeTransport(int employeeId, DateTime onSiteDate, CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(new DeleteEmployeeTransportRequest(employeeId, onSiteDate), cancellationToken);
            return Ok(response);
        }




        [HttpDelete("deletetransportbulk")]
        public async Task<ActionResult> DeleteEmployeeTransportBulk(DeleteEmployeeTransportBulkRequest request, CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(request, cancellationToken);
            return Ok(response);
        }


        [HttpGet("{Id}")]
        public async Task<ActionResult<GetEmployeeResponse>> Get(int Id, CancellationToken cancellationToken, [FromQuery] bool clearCache = false)
        {
            var response = await _mediator.Send(new GetEmployeeRequest(Id), cancellationToken);
            return Ok(response);

            //var outData = new GetEmployeeResponse();
            //string cacheEntityName = $"Employee_{Id}";

            //if (clearCache)
            //{
            //    _memoryCache.Remove($"API::{cacheEntityName}");
            //}

            //if (_memoryCache.TryGetValue($"API::{cacheEntityName}", out outData))
            //{
            //    return Ok(outData);
            //}
            //else
            //{
            //    var response = await _mediator.Send(new GetEmployeeRequest(Id), cancellationToken);

            //    _memoryCache.Set($"API::{cacheEntityName}", response, TimeSpan.FromMinutes(GlobalConstants.ENDPOINT_TAS_PROFILE_CACHE_MINUTE));
            //    return Ok(response);
            //}
        }


        [HttpGet("profile/{Id}")]
        public async Task<ActionResult<GetEmployeeResponse>> GetProfile(int Id, CancellationToken cancellationToken)
        {
            //var response = await _mediator.Send(new GetEmployeeRequest(Id), cancellationToken);
            //    return Ok(response);

            var outData = new GetEmployeeResponse();
            string cacheEntityName = $"Employee_{Id}";

            if (_memoryCache.TryGetValue($"API::{cacheEntityName}", out outData))
            {
                return Ok(outData);
            }
            else
            {
                var response = await _mediator.Send(new GetEmployeeRequest(Id), cancellationToken);

                _memoryCache.Set($"API::{cacheEntityName}", response, TimeSpan.FromMinutes(GlobalConstants.ENDPOINT_TAS_PROFILE_CACHE_MINUTE));
                return Ok(response);
            }
        }


        [HttpGet("accounthistory/{employeeId}")]
        public async Task<ActionResult> GetAccountHistory(int employeeId, CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(new GetEmployeeAccountHistoryRequest(employeeId), cancellationToken);
            return Ok(response);
        }




        [HttpGet]
        public async Task<ActionResult<List<GetAllEmployeeResponse>>> GetAll(int? active, CancellationToken cancellationToken)
        {
            if (active == null)
            {
                var response = await _mediator.Send(new GetAllEmployeeRequest(null), cancellationToken);
                return Ok(response);
            }
            else
            {
                var response = await _mediator.Send(new GetAllEmployeeRequest(active), cancellationToken);
                return Ok(response);

            }
        }

        [HttpPost]
        public async Task<ActionResult> Create([FromForm] CreateEmployeeRequest request,
            CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(request, cancellationToken);
            return Ok(response);
        }

        [HttpPost("request")]
        public async Task<ActionResult> CreateRequest([FromForm] CreateEmployeeRequestRequest request,
    CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(request, cancellationToken);
            return Ok(response);
        }

        [HttpPost("statusdates")]
        public async Task<ActionResult> GetStatusDate(StatusEmployeeRequest request,
            CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(request, cancellationToken);
            return Ok(response);
        }

        [HttpPatch]
        public async Task<ActionResult> Update([FromForm] UpdateEmployeeRequest request,
            CancellationToken cancellationToken)
        {
            
            var response = await _mediator.Send(request, cancellationToken);

            string cacheEntityName = $"Employee_{request.Id}";
            _memoryCache.Remove($"API::{cacheEntityName}");
          //  _hTTPUserRepository.ClearAllEmployeeCache();
            return Ok(response);
        }

        [HttpDelete]
        public async Task<ActionResult> Delete(DeleteEmployeeRequest request,
            CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(request, cancellationToken);
            return Ok(response);
        }


        [HttpPost("rosterexecute")]
        public async Task<ActionResult> RosterExcecute(RosterExecuteEmployeeRequest request,
            CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(request, cancellationToken);
            string cacheEntityName = $"Employee_{request.EmployeeId}";
            _memoryCache.Remove($"API::{cacheEntityName}");
        //    _hTTPUserRepository.ClearAllEmployeeCache();

            return Ok(response);
        }


        [HttpPost("rosterexecutepreview")]
        public async Task<ActionResult<RosterExecutePreviewEmployeeResponse>> RosterExcecute(RosterExecutePreviewEmployeeRequest request,
            CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(request, cancellationToken);

            return Ok(response);
        }


        [HttpPost("checkAdAccount")]
        public async Task<ActionResult<CheckADAccountEmployeeResponse>> CheckAdAccount(CheckADAccountEmployeeRequest request,
    CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(request, cancellationToken);

            return Ok(response);
        }

        [HttpGet("myinfo")]
        public async Task<ActionResult> MyInfo(CancellationToken cancellationToken)
        {
            string? username = HttpContext.User.Identity.Name;
            ActiveDirectoryService ac = new ActiveDirectoryService();
            var user = ac.GetUserFromAd(_configuration.GetSection("AppSettings:Domain").Value, username);
            var data = await _mediator.Send(new LoginUserRequest(user.UserName));
            return Ok(data);
        }


        [HttpPost("bulkrequest")]
        public async Task<ActionResult> BulkRequest(BulkDownloadEmployeeRequest request,
CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(request, cancellationToken);
            //return File(response.ExcelFile, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"TAS_Room_Bulk_Download_{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}.xlsx"); ;

           return File(response.ExcelFile, "application/force-download", $"TAS_Employee_Bulk_Download_{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}.xlsx"); ;
      
        
        }


        [HttpPost("bulkgrouprequest")]
        public async Task<ActionResult> BulkGroupRequest(BulkDownloadGroupEmployeeRequest request,
CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(request, cancellationToken);
            //return File(response.ExcelFile, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"TAS_Room_Bulk_Download_{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}.xlsx"); ;
            return File(response.ExcelFile, "application/force-download", $"TAS_Employee_Bulk_Group_Download_{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}.xlsx"); ;


        }





        [HttpPost("bulkupload")]
        public async Task<ActionResult> BulkUpload([FromForm] BulkUploadEmployeeRequest request,
    CancellationToken cancellationToken)
        {
            var response =  await _mediator.Send(request, cancellationToken);
            _hTTPUserRepository.ClearAllEmployeeCache();
            return Ok(response);
        }


        [HttpPost("bulkuploadpreview")]
        public async Task<ActionResult> BulkUploadPreview([FromForm] BulkUploadPreviewEmployeeRequest request,
CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(request, cancellationToken);
            return Ok(response);
        }


        [HttpPost("bulkuploadgroup")]
        public async Task<ActionResult> BulkUploadGroup([FromForm] BulkUploadEmployeeGroupRequest request,
CancellationToken cancellationToken)
        {
            await _mediator.Send(request, cancellationToken);
            _hTTPUserRepository.ClearAllEmployeeCache();
            return Ok();
        }

        [HttpPost("bulkuploadpreviewgroup")]
        public async Task<ActionResult> BulkUploadPreviewGroup([FromForm] BulkUploadPreviewEmployeeGroupRequest request,
CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(request, cancellationToken);
            return Ok(response);
        }



        [HttpPut("changedata/costcode")]
        public async Task<ActionResult> ChangeCostCode(List<ChangeEmployeeData> request,
CancellationToken cancellationToken)
        {
            await _mediator.Send(new ChangeEmployeeDataRequest(ChangeEmployeeDataType.CostCode, request), cancellationToken);
            _hTTPUserRepository.ClearAllEmployeeCache();
            return Ok();
        }



        [HttpPut("changedata/location")]
        public async Task<ActionResult> ChangeLocation(ChangeEmployeeLocationRequest request,
CancellationToken cancellationToken)
        {
            await _mediator.Send(request, cancellationToken);
            _hTTPUserRepository.ClearAllEmployeeCache();
            return Ok();
        }



        [HttpPut("changedata/department")]
        public async Task<ActionResult> ChangeDepartment(List<ChangeEmployeeData> request, CancellationToken cancellationToken)
        {
            await _mediator.Send(new ChangeEmployeeDataRequest(ChangeEmployeeDataType.Department, request), cancellationToken);
            _hTTPUserRepository.ClearAllEmployeeCache();
            return Ok();
        }


        [HttpPut("changedata/employer")]
        public async Task<ActionResult> ChangeEmployer(List<ChangeEmployeeData> request,
CancellationToken cancellationToken)
        {
            await _mediator.Send(new ChangeEmployeeDataRequest(ChangeEmployeeDataType.Employer, request), cancellationToken);
            _hTTPUserRepository.ClearAllEmployeeCache();

            return Ok();
        }


        [HttpPut("changedata/position")]
        public async Task<ActionResult> ChangePosition(List<ChangeEmployeeData> request,
CancellationToken cancellationToken)
        {
            await _mediator.Send(new ChangeEmployeeDataRequest(ChangeEmployeeDataType.Position, request), cancellationToken);
            _hTTPUserRepository.ClearAllEmployeeCache();
            return Ok();
        }

        //        public async Task ChangeEmployeeDataGroup(ChangeEmployeeDataGroupRequest request, CancellationToken cancellationToken)

        [HttpPut("changedata/group")]
        public async Task<ActionResult> ChangeEmployeeDataGroup(ChangeEmployeeDataGroupRequest request,
CancellationToken cancellationToken)
        {
            await _mediator.Send(request, cancellationToken);
            _hTTPUserRepository.ClearAllEmployeeCache();
            return Ok();
        }

        [HttpGet("deactive/check/{empId}/{leaveDate}")]
        public async Task<ActionResult> EmployeeDeActiveDateCheck(int empId, DateTime leaveDate,
        CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(new EmployeeDeActiveDateCheckRequest(empId, leaveDate), cancellationToken);
            return Ok(response);
        }


        [HttpPut("deactive/check/multiple")]
        public async Task<ActionResult> EmployeeDeActiveDateCheckMultiple(EmployeeDeActiveDateCheckMultipleRequest request,
CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(request, cancellationToken);
            return Ok(response);
        }

        [HttpPost("deactive")]
        public async Task<ActionResult<List<DeActiveEmployeeResponse>>> DeActiveEmployee(DeActiveEmployeeRequest request,   
        CancellationToken cancellationToken)
        {
           var response =  await _mediator.Send(request, cancellationToken);
            _hTTPUserRepository.ClearAllEmployeeCache();
            return Ok(response);
        }


        [HttpPost("reactive")]
        public async Task<ActionResult> ReActiveEmployee(ReActiveEmployeeRequest request,
        CancellationToken cancellationToken)
        {
            await _mediator.Send(request, cancellationToken);
            _hTTPUserRepository.ClearAllEmployeeCache();
            //foreach (var item in request.Employees)
            //{
            //    string cacheEntityName = $"Employee_{item.EmployeeId}";
            //    _memoryCache.Remove($"API::{cacheEntityName}");
            //}

            return Ok();
        }



        [HttpDelete("removepassportimage")]
        public async Task<ActionResult> RemovePassportImage(RemovePassportImageEmployeeRequest request,
        CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(request, cancellationToken);
            return Ok(response);
        }


    }
}
