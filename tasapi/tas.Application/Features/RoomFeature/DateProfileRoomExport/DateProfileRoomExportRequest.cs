using MediatR;
using tas.Application.Features.RoomFeature.DateProfileRoomExport;
using tas.Domain.Common;

namespace tas.Application.Features.RoomFeature.DateProfileRoomExport
{
    public sealed record DateProfileRoomExportRequest(

            DateTime CurrentDate,
            int RoomId,
            List<int> PeopleTypeIds

        ) : IRequest<DateProfileRoomExportResponse>;


}
