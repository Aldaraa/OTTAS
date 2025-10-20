using MediatR;
using tas.Application.Features.RoomFeature.RoomAssignmentEmployeeInfo;

namespace tas.Application.Features.RoomFeature.RoomAssignmentEmployeeInfo
{
    public sealed record RoomAssignmentEmployeeInfoRequest(int EmployeeId) : IRequest<RoomAssignmentEmployeeInfoResponse>;
}
