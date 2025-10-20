using AutoMapper;
using tas.Domain.Entities;

namespace tas.Application.Features.GroupMasterFeature.GetAllGroupMaster
{
    public sealed class GetAllGroupMasterMapper : Profile
    {
        public GetAllGroupMasterMapper()
        {
            CreateMap<GroupMaster, GetAllGroupMasterResponse>();
        }
    }

}

