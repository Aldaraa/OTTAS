using AutoMapper;
using tas.Domain.Entities;

namespace tas.Application.Features.RosterFeature.GetAllRoster
{
    public sealed class GetAllRosterMapper : Profile
    {
        public GetAllRosterMapper()
        {
            CreateMap<Roster, GetAllRosterResponse>();
        }
    }

}

