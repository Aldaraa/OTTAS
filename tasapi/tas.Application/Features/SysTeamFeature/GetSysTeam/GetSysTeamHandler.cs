using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.SysTeamFeature.GetAllSysTeam;
using tas.Application.Repositories;

namespace tas.Application.Features.SysTeamFeature.GetSysTeam
{

    public sealed class GetSysTeamHandler : IRequestHandler<GetSysTeamRequest, GetSysTeamResponse>
    {
        private readonly ISysTeamRepository _SysTeamRepository;
        private readonly IMapper _mapper;

        public GetSysTeamHandler(ISysTeamRepository SysTeamRepository, IMapper mapper)
        {
            _SysTeamRepository = SysTeamRepository;
            _mapper = mapper;
        }

        public async Task<GetSysTeamResponse> Handle(GetSysTeamRequest request, CancellationToken cancellationToken)
        {
            var SysTeams = await _SysTeamRepository.GetSysTeamProfile(request.Id, cancellationToken);
            return SysTeams;

        }
    }
}
