using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Repositories;

namespace tas.Application.Features.GroupMasterFeature.GetProfileGroupMaster
{

    public sealed class GetProfileGroupMasterHandler : IRequestHandler<GetProfileGroupMasterRequest, List<GetProfileGroupMasterResponse>>
    {
        private readonly IGroupMasterRepository _GroupMasterRepository;
        private readonly IMapper _mapper;

        public GetProfileGroupMasterHandler(IGroupMasterRepository GroupMasterRepository, IMapper mapper)
        {
            _GroupMasterRepository = GroupMasterRepository;
            _mapper = mapper;
        }

        public async Task<List<GetProfileGroupMasterResponse>> Handle(GetProfileGroupMasterRequest request, CancellationToken cancellationToken)
        {
            var GroupMasters = await _GroupMasterRepository.ProfileData(request, cancellationToken);
            return _mapper.Map<List<GetProfileGroupMasterResponse>>(GroupMasters);

        }
    }
}
