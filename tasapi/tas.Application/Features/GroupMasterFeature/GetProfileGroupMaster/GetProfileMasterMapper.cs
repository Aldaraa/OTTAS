using AutoMapper;
using tas.Domain.Entities;

namespace tas.Application.Features.GroupMasterFeature.GetProfileGroupMaster
{
    public sealed class GetProfileGroupMasterMapper : Profile
    {
        public GetProfileGroupMasterMapper()
        {
            CreateMap<GroupMaster, GetProfileGroupMasterResponse>();
        }
    }

}

