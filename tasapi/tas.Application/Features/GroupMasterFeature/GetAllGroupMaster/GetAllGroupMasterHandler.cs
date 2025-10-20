using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.UserFeatures.GetAllUser;
using tas.Application.Repositories;

namespace tas.Application.Features.GroupMasterFeature.GetAllGroupMaster
{

    public sealed class GetAllGroupMasterHandler : IRequestHandler<GetAllGroupMasterRequest, List<GetAllGroupMasterResponse>>
    {
        private readonly IGroupMasterRepository _GroupMasterRepository;
        private readonly IMapper _mapper;

        public GetAllGroupMasterHandler(IGroupMasterRepository GroupMasterRepository, IMapper mapper)
        {
            _GroupMasterRepository = GroupMasterRepository;
            _mapper = mapper;
        }

        public async Task<List<GetAllGroupMasterResponse>> Handle(GetAllGroupMasterRequest request, CancellationToken cancellationToken)
        {
            return await _GroupMasterRepository.GetAllData(request, cancellationToken);
        }
    }
}
