using AutoMapper;
using tas.Domain.Entities;

namespace tas.Application.Features.SysTeamFeature.GetAllSysTeam
{
    public sealed class GetAllSysTeamMapper : Profile
    {
        public GetAllSysTeamMapper()
        {
            CreateMap<SysTeam, GetAllSysTeamResponse>();
        }
    }

}

