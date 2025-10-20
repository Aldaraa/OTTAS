using MediatR;
using tas.Application.Features.RoomFeature.ActiveSearchRoom;
using tas.Domain.Common;

namespace tas.Application.Features.RoomFeature.ActiveSearchRoom
{
    public sealed class ActiveSearchRoomRequest : IRequest<ActiveSearchRoomResponse>
    {
            public RequestSearchRoomModel model { get; set; }
            public SortOptions? sort { get; set; }
            public int pageIndex { get; set; }
            public int pageSize { get; set; }
    }



public record RequestSearchRoomModel(

        DateTime startDate,
        DateTime endDate,
        int? CampId,
        int? RoomTypeId,
        int? Private,
        int? bedCount,
        string? RoomNumber,
        int? Locked,
        int? hasOwner
    );


    public class SortOptions
    {
        public string SortBy { get; set; }
        public string SortDirection { get; set; }
    }


}


//    public sealed record ActiveSearchRoomRequest(
//            DateTime startDate, 
//            DateTime endDate,
//            int CampId,
//            int? RoomTypeId,
//            int? Private,
//            int? activeBedCount,
//            string? RoomNumber

//        ) : IRequest<List<ActiveSearchRoomResponse>>;
//}
