using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.RoomTypeFeature.CreateRoomType
{
    public sealed record CreateRoomTypeRequest(string Description, int Active) : IRequest;
}
