using AutoMapper;
using tas.Domain.Entities;

namespace tas.Application.Features.SysTeamFeature.GetSysTeam
{
    public sealed class GetSysTeamMapper : Profile
    {
        public GetSysTeamMapper()
        {
            CreateMap<SysTeam, GetSysTeamResponse>();
        }
    }

}

