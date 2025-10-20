using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Domain.Entities;

namespace tas.Application.Features.RoomFeature.CreateRoom
{
    public sealed class CreateRoomMapper : Profile
    {
        public CreateRoomMapper()
        {
            CreateMap<CreateRoomRequest, Room>();
        }
    }
}
