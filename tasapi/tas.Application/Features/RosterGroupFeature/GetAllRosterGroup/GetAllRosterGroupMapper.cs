using AutoMapper;
using tas.Domain.Entities;

namespace tas.Application.Features.RosterGroupFeature.GetAllRosterGroup
{
    public sealed class GetAllRosterGroupMapper : Profile
    {
        public GetAllRosterGroupMapper()
        {
            CreateMap<RosterGroup, GetAllRosterGroupResponse>();
        }
    }

}

