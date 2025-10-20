using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Domain.Entities;

namespace tas.Application.Features.RoomTypeFeature.UpdateRoomType
{

    public sealed class UpdateRoomTypeMapper : Profile
    {
        public UpdateRoomTypeMapper()
        {
            CreateMap<UpdateRoomTypeRequest, RoomType>();
        }
    }
}
