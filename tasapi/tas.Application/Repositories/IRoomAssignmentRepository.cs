using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.GroupMembersFeature.CreateGroupMembers;
using tas.Application.Features.RoomAssignmentFeature.CreateRoomAssignment;
using tas.Application.Features.RoomAssignmentFeature.CreateRoomAssignmentOwnership;
using tas.Application.Features.RoomAssignmentFeature.FindAvailableByDatesAssignment;
using tas.Application.Features.RoomFeature.CreateRoomAssignment;
using tas.Application.Features.RoomFeature.CreateRoomAssignmentOwnership;
using tas.Application.Features.RoomFeature.GetAllActiveRoomAssignment;
using tas.Application.Features.RoomFeature.RemoveRoomAssignmentOwnership;
using tas.Application.Features.RoomFeature.RoomAssignmentEmployeeInfo;
using tas.Domain.Entities;

namespace tas.Application.Repositories
{

    public interface IRoomAssignmentRepository : IBaseRepository<RoomAssignment>
    {

        Task<CreateRoomAssignmentResponse> SaveTemporaryRoom(CreateRoomAssignmentRequest request, CancellationToken cancellationToken);

        Task<List<GetAllActiveRoomAssignmentResponse>> GetAllActiveRooms(GetAllActiveRoomAssignmentRequest request, CancellationToken cancellationToken);
        Task<RoomAssignmentEmployeeInfoResponse> EmployeeInfo(RoomAssignmentEmployeeInfoRequest request, CancellationToken cancellationToken);

        Task<List<FindAvailableByDatesAssignmentResponse>> FindAvailableByDatesAssignment(FindAvailableByDatesAssignmentRequest request, CancellationToken cancellationToken);

        Task<List<CreateRoomAssignmentOwnershipResponse>> SaveOwnershipRoom(CreateRoomAssignmentOwnershipRequest request, CancellationToken cancellationToken);

        Task RemoveOwnershipRoom(RemoveRoomAssignmentOwnershipRequest request, CancellationToken cancellationToken);



    }
}
