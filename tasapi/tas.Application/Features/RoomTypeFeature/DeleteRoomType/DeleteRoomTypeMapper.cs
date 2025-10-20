using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.RoomTypeFeature.UpdateRoomType;
using tas.Domain.Entities;

namespace tas.Application.Features.RoomTypeFeature.DeleteRoomType
{

    public sealed class DeleteRoomTypeMapper : Profile
    {
        public DeleteRoomTypeMapper()
        {
            CreateMap<DeleteRoomTypeRequest, RoomType>();
        }
    }
}
