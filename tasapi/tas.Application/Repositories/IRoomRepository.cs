using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.BedFeature.GetBed;
using tas.Application.Features.RoomAssignmentFeature.FindAvailableByDatesAssignment;
using tas.Application.Features.RoomFeature.ActiveSearchRoom;
using tas.Application.Features.RoomFeature.AssignRoomDateOccupancyAnalyze;
using tas.Application.Features.RoomFeature.BulkDownloadRoom;
using tas.Application.Features.RoomFeature.BulkUploadPreviewRoom;
using tas.Application.Features.RoomFeature.BulkUploadRoom;
using tas.Application.Features.RoomFeature.CreateRoom;
using tas.Application.Features.RoomFeature.DateProfileRoom;
using tas.Application.Features.RoomFeature.DateProfileRoomDetail;
using tas.Application.Features.RoomFeature.DateProfileRoomExport;
using tas.Application.Features.RoomFeature.DateStatusRoom;
using tas.Application.Features.RoomFeature.EmployeeRoomProfile;
using tas.Application.Features.RoomFeature.FindAvailableByDates;
using tas.Application.Features.RoomFeature.FindAvailableByRoomId;
using tas.Application.Features.RoomFeature.FindAvailableRoom;
using tas.Application.Features.RoomFeature.FindRoomDateOccupancyAnalyze;
using tas.Application.Features.RoomFeature.GetAllRoom;
using tas.Application.Features.RoomFeature.GetRoom;
using tas.Application.Features.RoomFeature.GetRoomAssignAvialable;
using tas.Application.Features.RoomFeature.GetVirtualRoom;
using tas.Application.Features.RoomFeature.MonthStatusRoom;
using tas.Application.Features.RoomFeature.MonthStatusRoomOwner;
using tas.Application.Features.RoomFeature.SearchRoom;
using tas.Application.Features.RoomFeature.UpdateRoom;
using tas.Application.Features.RoomOwnerAndLockFeature.DateProfileRoomOwnerAndLock;
using tas.Domain.Entities;

namespace tas.Application.Repositories
{
    public interface IRoomRepository : IBaseRepository<Room>
    {
        Task<List<GetAllRoomResponse>> GetAll(GetAllRoomRequest request, CancellationToken cancellationToken);

        Task<GetRoomResponse> Get(int Id, CancellationToken cancellationToken);

        Task GenerateBed(int roomId);

        Task<ActiveSearchRoomResponse> StatusBetweenDates(ActiveSearchRoomRequest request, CancellationToken cancellationToken);

        Task<DateStatusRoomResponse> GetRoomDateStatus(DateStatusRoomRequest request, CancellationToken cancellationToken);

        Task<List<FindAvailableRoomResponse>> FindAvailableRooms(FindAvailableRoomRequest request, CancellationToken cancellationToken);

        Task CreateValidateRoom(CreateRoomRequest request, CancellationToken cancellationToken);

        Task UpdateValidateRoom(UpdateRoomRequest request, CancellationToken cancellationToken);

        Task<DateProfileRoomResponse> GetRoomDateProfile(DateProfileRoomRequest request, CancellationToken cancellationToken);

        Task<DateProfileRoomOwnerAndLockResponse> GetRoomOwnerAndLockDateProfile(DateProfileRoomOwnerAndLockRequest request, CancellationToken cancellationToken);

        Task<List<DateProfileRoomDetailResponse>> GetRoomDetailDateProfile(DateProfileRoomDetailRequest request, CancellationToken cancellationToken);



        Task<FindAvailableByRoomIdResponse> FindAvailableByRoomId(FindAvailableByRoomIdRequest request, CancellationToken cancellationToken);

        Task<BulkDownloadRoomResponse> BulkRequestDownload(BulkDownloadRoomRequest request, CancellationToken cancellationToken);


        Task BulkRequestUpload(BulkUploadRoomRequest request, CancellationToken cancellationToken);

        Task<BulkUploadPreviewRoomResponse> BulkRequestUploadPreview(BulkUploadPreviewRoomRequest request, CancellationToken cancellationToken);

        Task<GetVirtualRoomResponse> GetVirtualRoomId( CancellationToken cancellationToken);

        Task<MonthStatusRoomResponse> GetRoomMonthStatus(MonthStatusRoomRequest request, CancellationToken cancellationToken);

        Task<List<MonthStatusRoomOwnerResponse>> GetRoomMonthStatusOwner(MonthStatusRoomOwnerRequest request, CancellationToken cancellationToken);

        Task<List<FindAvailableByDatesResponse>> FindAvailableByDates(FindAvailableByDatesRequest request, CancellationToken cancellationToken);

        Task<List<GetRoomAssignAvialableResponse>> GetRoomAssignAvialable(GetRoomAssignAvialableRequest request, CancellationToken cancellationToken);



        Task<EmployeeRoomProfileResponse> GetEmployeeRoomProfile(EmployeeRoomProfileRequest request, CancellationToken cancellationToken);


        Task<FindRoomDateOccupancyAnalyzeResponse> FindRoomDateOccupancyAnalyze(FindRoomDateOccupancyAnalyzeRequest request, CancellationToken cancellationToken);

        Task<AssignRoomDateOccupancyAnalyzeResponse> AssignRoomDateOccupancyAnalyze(AssignRoomDateOccupancyAnalyzeRequest request, CancellationToken cancellationToken);



        Task<SearchRoomResponse> SearchRoom(SearchRoomRequest request, CancellationToken cancellationToken);

        Task UpdateRoomBedCount(int roomId);

        Task<DateProfileRoomExportResponse> GetRoomDateProfileExport(DateProfileRoomExportRequest request, CancellationToken cancellationToken);


        Task DeActiveRoomOwnersRemove(int RoomId, CancellationToken cancellationToken);
    }
}
