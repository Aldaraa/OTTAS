using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.RoomFeature.BulkDownloadRoom
{
    public sealed record BulkDownloadRoomRequest(List<int> RoomIds) : IRequest<BulkDownloadRoomResponse>;
}
