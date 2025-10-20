using MediatR;
using tas.Application.Features.RoomFeature.GetAllActiveRoomAssignment;

namespace tas.Application.Features.RoomFeature.GetAllActiveRoomAssignment
{
    public sealed record GetAllActiveRoomAssignmentRequest : IRequest<List<GetAllActiveRoomAssignmentResponse>>;
}
