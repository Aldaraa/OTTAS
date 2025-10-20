using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.RoomFeature.DeleteRoom
{
    public sealed record DeleteRoomRequest(int Id) : IRequest;
}
