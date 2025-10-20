using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.UserFeatures.GetAllUser;
using tas.Application.Repositories;

namespace tas.Application.Features.RosterGroupFeature.GetAllRosterGroup
{

    public sealed class GetAllRosterGroupHandler : IRequestHandler<GetAllRosterGroupRequest, List<GetAllRosterGroupResponse>>
    {
        private readonly IRosterGroupRepository _RosterGroupRepository;
        private readonly IMapper _mapper;

        public GetAllRosterGroupHandler(IRosterGroupRepository RosterGroupRepository, IMapper mapper)
        {
            _RosterGroupRepository = RosterGroupRepository;
            _mapper = mapper;
        }

        public async Task<List<GetAllRosterGroupResponse>> Handle(GetAllRosterGroupRequest request, CancellationToken cancellationToken)
        {
           return await _RosterGroupRepository.GetAllData(request, cancellationToken);

        }
    }
}
