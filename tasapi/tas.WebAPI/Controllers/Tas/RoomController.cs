using MediatR;
using Microsoft.AspNetCore.Mvc;
using tas.Application.Features.RoomFeature.GetAllRoom;
using tas.Application.Features.RoomFeature.ActiveSearchRoom;
using tas.Application.Features.RoomFeature.CreateRoom;
using tas.Application.Features.RoomFeature.DeleteRoom;
using tas.Application.Features.RoomFeature.GetRoom;
using tas.Application.Features.RoomFeature.UpdateRoom;
using tas.Application.Service;
using tas.Application.Features.RoomFeature.DateStatusRoom;
using tas.Application.Features.RoomFeature.FindAvailableRoom;
using tas.Application.Features.RoomFeature.DateProfileRoom;
using tas.Application.Features.RoomFeature.FindAvailableByRoomId;
using tas.Application.Features.RoomFeature.BulkDownloadRoom;
using tas.Application.Features.RoomFeature.BulkUploadRoom;
using tas.Application.Features.RoomFeature.GetVirtualRoom;
using tas.Application.Features.RoomFeature.BulkUploadPreviewRoom;
using tas.Application.Features.RoomFeature.MonthStatusRoom;
using tas.Application.Features.RoomFeature.FindAvailableByDates;
using DocumentFormat.OpenXml.Wordprocessing;
using tas.Application.Features.CampFeature.GetAllCamp;
using tas.Domain.Common;
using tas.Domain.Entities;
using Microsoft.Extensions.Caching.Memory;
using tas.Application.Features.RoomFeature.EmployeeRoomProfile;
using tas.Application.Features.RoomFeature.FindRoomDateOccupancyAnalyze;
using tas.Application.Features.EmployeeFeature.SearchEmployee;
using tas.Application.Features.RoomFeature.SearchRoom;
using tas.Application.Features.RoomFeature.GetRoomAssignAvialable;
using tas.Application.Features.RoomFeature.AssignRoomDateOccupancyAnalyze;
using tas.Application.Features.RoomFeature.DateProfileRoomDetail;
using tas.Application.Repositories;
using tas.Application.Features.RoomOwnerAndLockFeature.DateProfileRoomOwnerAndLock;
using tas.Application.Features.RoomFeature.MonthStatusRoomOwner;
using tas.Application.Features.RoomFeature.DateProfileRoomExport;

namespace tas.WebAPI.Controllers.Tas
{

    [Route("api/tas/[controller]")]
    [ApiController]
    [RoleAuthorize]
    public class RoomController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<RoomController> _logger;
        private readonly CacheService _memoryCache;
        private readonly HTTPUserRepository _hTTPUserRepository;

        public RoomController(IMediator mediator, ILogger<RoomController> logger, CacheService memoryCache, HTTPUserRepository hTTPUserRepository)
        {
            _mediator = mediator;
            _logger = logger;
            _memoryCache = memoryCache; 
            _hTTPUserRepository = hTTPUserRepository;
        }

        [HttpGet]
        //[RoleAuthorize("SystemAdmin,AccomAdmin")]
        public async Task<ActionResult<List<GetAllRoomResponse>>> GetAll(CancellationToken cancellationToken)
        {
            //  var httpContext = new HttpContextAccessor().HttpContext;
            //  var roles = httpContext.User.Claims.Where(c => c.Type == "role").Select(c => c.Value).ToList();


            var outData = new List<GetAllRoomResponse>();
            string cacheEntityName = typeof(Room).Name;

                if (_memoryCache.TryGetValue($"API::{cacheEntityName}", out outData))
                {
                    return Ok(outData);
                }
                else
                {

                    var response = await _mediator.Send(new GetAllRoomRequest(null), cancellationToken);
                    _memoryCache.Set($"API::{cacheEntityName}", response, TimeSpan.FromMinutes(GlobalConstants.ENDPOINT_MASTER_CACHE_MINUTE));
                    return Ok(response);

                }

        }


        [HttpGet("{Id}")]
        public async Task<ActionResult<GetRoomResponse>> Get(int Id, CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(new GetRoomRequest(Id), cancellationToken);
            return Ok(response);
        }


        [HttpPost("search")]
        public async Task<ActionResult<SearchRoomResponse>> Search(SearchRoomRequest request, string? sortBy,
        string? sortDirection, CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(request, cancellationToken);
            if (!string.IsNullOrEmpty(sortBy))
            {
                var property = typeof(SearchRoomRequest).GetProperty(sortBy);
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
            return Ok(response);
        }



        [HttpGet("employeeprofile/{Id}")]
        public async Task<ActionResult<EmployeeRoomProfileResponse>> GetEmployeeProfile(int Id, CancellationToken cancellationToken)
        {

            var outData = new List<EmployeeRoomProfileResponse>();
            string cacheEntityName = typeof(GetRoomResponse).Name;

            if (_memoryCache.TryGetValue($"API::{cacheEntityName}", out outData))
            {
                return Ok(outData);
            }
            else
            {
                var response = await _mediator.Send(new EmployeeRoomProfileRequest(Id), cancellationToken);
                _memoryCache.Set($"API::{cacheEntityName}", response, TimeSpan.FromMinutes(GlobalConstants.ENDPOINT_MASTER_CACHE_MINUTE));
                return Ok(response);

            }


        }


        [HttpPost]
        public async Task<ActionResult> Create(CreateRoomRequest request,
            CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(request, cancellationToken);
            _hTTPUserRepository.ClearAllMasterCache<Room>();
            //_memoryCache.Remove($"API::{typeof(Room).Name}_1");
            //_memoryCache.Remove($"API::{typeof(Room).Name}_0");
            //_memoryCache.Remove($"API::{typeof(Room).Name}");
            return Ok(response);
        }


        [HttpPost("statusbetweendates")]

        public async Task<ActionResult<ActiveSearchRoomResponse>> DisplayRoomStatusBetweenDates(ActiveSearchRoomRequest request,
            CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(request, cancellationToken);
            return Ok(response);
        }


        [HttpPost("findavailable")]
        public async Task<ActionResult<List<FindAvailableRoomResponse>>> FindAvailableRooms(FindAvailableRoomRequest request,
            CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(request, cancellationToken);
            return Ok(response);
        }


        [HttpPost("roomassignavialable")]
        public async Task<ActionResult<List<GetRoomAssignAvialableResponse>>> GetRoomAssignAvialable(GetRoomAssignAvialableRequest request,
            CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(request, cancellationToken);
            return Ok(response);
        }




        [HttpPost("findavailablebydates")]
        public async Task<ActionResult<List<FindAvailableByDatesResponse>>> FindAvailableByDates(FindAvailableByDatesRequest request,
    CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(request, cancellationToken);
            return Ok(response);
        }

        [HttpPost("findavailablebyroomid")]
        public async Task<ActionResult<List<FindAvailableRoomResponse>>> FindAvailableRoomID(FindAvailableByRoomIdRequest request,
    CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(request, cancellationToken);
            return Ok(response);
        }

        [HttpPost("datestatus")]
        public async Task<ActionResult<DateStatusRoomResponse>> DateStatusRooms(DateStatusRoomRequest request,
            CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(request, cancellationToken);
            return Ok(response);
        }

        [HttpPost("dateprofile")]
        public async Task<ActionResult<DateProfileRoomResponse>> DateProfileRoom(DateProfileRoomRequest request,
    CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(request, cancellationToken);
            return Ok(response);
        }

        [HttpPost("ownerandlockdateprofile")]
        public async Task<ActionResult<DateProfileRoomOwnerAndLockResponse>> DateProfileRoomOwnerAndLock(DateProfileRoomOwnerAndLockRequest request,
CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(request, cancellationToken);
            return Ok(response);
        }



        [HttpPost("detaildateprofile")]
        public async Task<ActionResult<DateProfileRoomDetailResponse>> DetailDateProfileRoom(DateProfileRoomDetailRequest request,
    CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(request, cancellationToken);
            return Ok(response);
        }



        [HttpPut]
        public async Task<ActionResult> Update(UpdateRoomRequest request,
            CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(request, cancellationToken);
            _hTTPUserRepository.ClearAllMasterCache<Room>();
            //_memoryCache.Remove($"API::{typeof(Room).Name}_1");
            //_memoryCache.Remove($"API::{typeof(Room).Name}_0");
            //_memoryCache.Remove($"API::{typeof(Room).Name}");
            return Ok(response);
        }

        [HttpDelete]
        public async Task<ActionResult> Delete(DeleteRoomRequest request,
            CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(request, cancellationToken);
            _hTTPUserRepository.ClearAllMasterCache<Room>();
            //_memoryCache.Remove($"API::{typeof(Room).Name}_1");
            //_memoryCache.Remove($"API::{typeof(Room).Name}_0");
            //_memoryCache.Remove($"API::{typeof(Room).Name}");
            return Ok(response);
        }

        [HttpPost("bulkrequest")]
        public async Task<ActionResult> BulkRequest(BulkDownloadRoomRequest request,
    CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(request, cancellationToken);
            string fileName = $"TAS_Room_Bulk_Download_{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}.xlsx";
            HttpContext.Response.Headers.Add("FileName", fileName);
            return File(response.ExcelFile, "application/force-download",fileName );;
        }


        [HttpPost("bulkupload")]
        public async Task<ActionResult> BulkUpload([FromForm] BulkUploadRoomRequest request,
    CancellationToken cancellationToken)
        {
            await _mediator.Send(request, cancellationToken);
            _hTTPUserRepository.ClearAllMasterCache<Room>();
            return Ok();
        }



        [HttpPost("bulkuploadpreview")]
        public async Task<ActionResult> BulkUploadPreview([FromForm] BulkUploadPreviewRoomRequest request,
    CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(request, cancellationToken);
            return Ok(response);
        }


        [HttpGet("getvirtualroomid")]
        public async Task<ActionResult<GetVirtualRoomResponse>> GetVirtualRoomId(CancellationToken cancellationToken)
        {
            //var response = await _mediator.Send(new GetVirtualRoomRequest(), cancellationToken);
            //return Ok(response);


            var outData = new GetVirtualRoomResponse();
            string cacheEntityName = "GetVirtualRoomId";

            if (_memoryCache.TryGetValue($"API::{cacheEntityName}", out outData))
            {
                return Ok(outData);
            }
            else
            {
                var response = await _mediator.Send(new GetVirtualRoomRequest(), cancellationToken);
                _memoryCache.Set($"API::{cacheEntityName}", response, TimeSpan.FromMinutes(GlobalConstants.ENDPOINT_MASTER_STATIC_MINUTE));
                return Ok(response);

            }




        }


        //   public async Task<List<MonthStatusRoomResponse>> GetRoomMonthStatus(MonthStatusRoomRequest request, CancellationToken cancellationToken)

        [HttpGet("getmonthroomdata/owner/{currentDate}/{roomId}")]
        public async Task<ActionResult<MonthStatusRoomOwnerResponse>> GetRoomMonthStatusOwner(DateTime currentDate, int roomId, CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(new MonthStatusRoomOwnerRequest(currentDate, roomId), cancellationToken);
            return Ok(response);
        }


        [HttpPost("getmonthroomdata")]
        public async Task<ActionResult<MonthStatusRoomResponse>> GetRoomMonthStatusOwner(MonthStatusRoomRequest request, CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(request, cancellationToken);
            return Ok(response);
        }



        [HttpPost("analyze")]
        public async Task<ActionResult<FindRoomDateOccupancyAnalyzeResponse>> FindRoomDateOccupancyAnalyze(FindRoomDateOccupancyAnalyzeRequest request, CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(request, cancellationToken);
            return Ok(response);
        }



        [HttpPost("assignanalyze")]
        public async Task<ActionResult<AssignRoomDateOccupancyAnalyzeResponse>> AssignRoomDateOccupancyAnalyze(AssignRoomDateOccupancyAnalyzeRequest request, CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(request, cancellationToken);
            return Ok(response);
        }

        [HttpPost("dateprofileexport")]
        public async Task<ActionResult> DetailDateProfileRoomExport(DateProfileRoomExportRequest request, CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(request, cancellationToken);
            if (response.ExcelFile != null)
            {
                return File(response.ExcelFile, "application/force-download", $"TAS_RoomProfile_Download_{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}.xlsx"); ;
            }
            else
            {
                return NoContent();
            }
        }
    }
}
