using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.UserFeatures.GetAllUser;
using tas.Application.Repositories;

namespace tas.Application.Features.GroupDetailFeature.GetAllGroupDetail
{

    public sealed class GetAllGroupDetailHandler : IRequestHandler<GetAllGroupDetailRequest, GetAllGroupDetailResponse>
    {
        private readonly IGroupDetailRepository _GroupDetailRepository;
        private readonly IMapper _mapper;

        public GetAllGroupDetailHandler(IGroupDetailRepository GroupDetailRepository, IMapper mapper)
        {
            _GroupDetailRepository = GroupDetailRepository;
            _mapper = mapper;
        }

        public async Task<GetAllGroupDetailResponse> Handle(GetAllGroupDetailRequest request, CancellationToken cancellationToken)
        {
                return await _GroupDetailRepository.GetAllByGroupMasterId(request.status, request.GroupMasterId, cancellationToken);
           

        }
    }
}
