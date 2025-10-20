using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.UserFeatures.GetAllUser;
using tas.Application.Repositories;

namespace tas.Application.Features.SysTeamFeature.GetAllSysTeam
{

    public sealed class GetAllSysTeamHandler : IRequestHandler<GetAllSysTeamRequest, List<GetAllSysTeamResponse>>
    {
        private readonly ISysTeamRepository _SysTeamRepository;
        private readonly IMapper _mapper;

        public GetAllSysTeamHandler(ISysTeamRepository SysTeamRepository, IMapper mapper)
        {
            _SysTeamRepository = SysTeamRepository;
            _mapper = mapper;
        }

        public async Task<List<GetAllSysTeamResponse>> Handle(GetAllSysTeamRequest request, CancellationToken cancellationToken)
        {
                var systemTeams = await _SysTeamRepository.GetAllSysTeam(cancellationToken);
                return _mapper.Map<List<GetAllSysTeamResponse>>(systemTeams);
           

        }
    }
}
