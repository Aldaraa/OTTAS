using MediatR;
using tas.Application.Features.RoomFeature.EmployeeRoomProfile;

namespace tas.Application.Features.RoomFeature.EmployeeRoomProfile
{
    public sealed record EmployeeRoomProfileRequest(int Id) : IRequest<EmployeeRoomProfileResponse>;
}
