using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.RoomFeature.UpdateRoom;
using tas.Domain.Entities;

namespace tas.Application.Features.RoomFeature.DeleteRoom
{

    public sealed class DeleteRoomMapper : Profile
    {
        public DeleteRoomMapper()
        {
            CreateMap<DeleteRoomRequest, Room>();
        }
    }
}
