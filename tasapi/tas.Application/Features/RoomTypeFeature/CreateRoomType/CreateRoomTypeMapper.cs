using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Domain.Entities;

namespace tas.Application.Features.RoomTypeFeature.CreateRoomType
{
    public sealed class CreateRoomTypeMapper : Profile
    {
        public CreateRoomTypeMapper()
        {
            CreateMap<CreateRoomTypeRequest, RoomType>();
        }
    }
}
