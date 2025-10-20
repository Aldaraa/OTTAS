using AutoMapper;
using tas.Domain.Entities;

namespace tas.Application.Features.RosterFeature.GetRoster
{
    public sealed class GetRosterMapper : Profile
    {
        public GetRosterMapper()
        {
            CreateMap<Roster, GetRosterResponse>();
        }
    }

}

