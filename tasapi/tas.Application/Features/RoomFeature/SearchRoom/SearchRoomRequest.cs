using MediatR;
using tas.Application.Features.RoomFeature.SearchRoom;
using tas.Domain.Common;

namespace tas.Application.Features.RoomFeature.SearchRoom
{
    public sealed record SearchRoomRequest(SearchRoomRequestModel model) : BasePagenationRequest, IRequest<SearchRoomResponse>;

    public sealed record SearchRoomRequestModel
    ( 
        int? CampId,
        int? RoomTypeId,
        string? RoomNumber,
        int? Private
        
    );

}




