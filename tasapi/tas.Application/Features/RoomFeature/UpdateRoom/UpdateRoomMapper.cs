using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Domain.Entities;

namespace tas.Application.Features.RoomFeature.UpdateRoom
{

    public sealed class UpdateRoomMapper : Profile
    {
        public UpdateRoomMapper()
        {
            CreateMap<UpdateRoomRequest, Room>();
        }
    }
}
